using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FPS_Controller : MonoBehaviour
{
    public bool CanMoveJump { get; private set; } = true;
    public bool CanCrouch { get; private set; } = true;
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;
    public bool CanLook {  get; private set; } = true;

    ///References
    private CharacterController characterController;
    private Camera fpCamera;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3 (0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);

    [Header("Jump Paramaters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = 9.81f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float upDownClampRange = 80.0f;

    [Header("Inputs Customisation")]
    ///Movement
    [SerializeField] private string horizontalMoveInput = "Horizontal";
    [SerializeField] private string verticalMoveInput = "Vertical";
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    ///Look 
    [SerializeField] private string mouseXInput = "Mouse X";
    [SerializeField] private string mouseYInput = "Mouse Y";

    [Header("Sounds")]
    [SerializeField] private AudioSource audioSource;

    ///Footsteps
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float walksStepInterval = 0.6f;
    [SerializeField] private float crouchStepInterval = 0.9f;
    [SerializeField] private float sprintStepInterval = 0.3f;
    [SerializeField] private float velocityThreshold = 1.0f;

    ///Variables
    ///Movement
    private Vector3 currentMovement = Vector3.zero;
    private bool isMoving;
    private bool isCrouching;
    private bool duringCrouchAnimation;
    public float Magnitude;

    ///Look
    private float verticalRotation;

    ///Sounds
    private float nextStepTime;
    private int lastPlayedIndex = -1;

    private void OnEnable()
    {
        characterController = GetComponent<CharacterController>();
        fpCamera = Camera.main;
    }
    private void OnDisable()
    {
        
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        Magnitude = characterController.velocity.magnitude;

        if (CanMoveJump) HandleMovement();

        if (CanCrouch) HandleCrouching();

        if (CanLook) HandleRotation();

        HandleFootsteps();
    }

    private void HandleMovement()
    {
        float verticalInput = Input.GetAxis(verticalMoveInput);
        float horizontalInput = Input.GetAxis(horizontalMoveInput);

        float speedMutiplier = isCrouching ? 1.0f : Input.GetKey(sprintKey) ? sprintMultiplier : 1.0f;

        float horizontalSpeed = horizontalInput * (isCrouching ? crouchSpeed : walkSpeed) * speedMutiplier;
        float verticalSpeed = verticalInput * (isCrouching ? crouchSpeed : walkSpeed) * speedMutiplier;

        Vector3 horizontalMovement = new Vector3 (horizontalSpeed, 0, verticalSpeed);
        horizontalMovement = transform.rotation * horizontalMovement;

        HandleGravityAndJumping();

        currentMovement.x = horizontalMovement.x;
        currentMovement.z = horizontalMovement.z;


        characterController.Move(currentMovement * Time.deltaTime);

        isMoving = verticalInput != 0 || horizontalInput != 0;
    }
    private void HandleGravityAndJumping() 
    {
        if (characterController.isGrounded && !isCrouching)
        {
            currentMovement.y = -0.5f;

            if (!Physics.Raycast(fpCamera.transform.position, Vector3.up, 1) && Input.GetKey(jumpKey))
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y -= gravity * Time.deltaTime;
        }
    }
    private void HandleCrouching()
    {
        if (ShouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }
    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(fpCamera.transform.position, Vector3.up, 1)) yield break;

        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        //Mientras tiempoElapsed sea menor que el tiempo para agacharse va a hacerse la animacion de bajar el centro y la altura del personaje
        while (timeElapsed < timeToCrouch)
        {
            //Bajada/Subida progresiva desde current a target en determinado tiempo
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, currentCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }
    private void HandleRotation()
    {
        float mouseXRotation = Input.GetAxis(mouseXInput) * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= Input.GetAxis(mouseYInput) * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownClampRange, upDownClampRange);
        fpCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
    private void HandleFootsteps()
    {
        float currentStepInterval = ( isCrouching ? crouchStepInterval : Input.GetKey(sprintKey) ? sprintStepInterval : walksStepInterval);

        if (characterController.isGrounded && isMoving && Time.time > nextStepTime && characterController.velocity.magnitude > velocityThreshold)
        {
            PlayFootstepSounds();
            nextStepTime = Time.time + currentStepInterval;
        }
    }
    private void PlayFootstepSounds()
    {
        int randomIndex;
        if (footstepSounds.Length == 1)
        {
            randomIndex = 0;
        }
        else
        {
            randomIndex = Random.Range(0, footstepSounds.Length - 1);
            if (randomIndex >= lastPlayedIndex)
            {
                randomIndex++;
            }
        }

        lastPlayedIndex = randomIndex;
        audioSource.clip = footstepSounds[randomIndex];
        audioSource.Play();
    }
}
