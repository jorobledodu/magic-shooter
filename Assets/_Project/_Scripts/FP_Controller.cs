using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FP_Controller : MonoBehaviour
{
    public bool CanMove { get; set; } = true;
    public bool CanCrouch { get; set; } = true;
    public bool CanRun { get; set; } = true;
    public bool CanJump { get; set; } = true;
    private bool ShouldRun => CanRun && _inputHandle.RunTriggered;
    private bool ShouldCrouch => _inputHandle.CrouchTriggered && !duringCrouchingAnimation && _characterController.isGrounded;
    private bool ShouldJump => _inputHandle.JumpTriggered && _characterController.isGrounded;

    [Header("Movement Parametres")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float runSpeed = 6.0f; //No es necesario modificarlos ya que se calculan por equaciones
    [SerializeField] private float crouchSpeed = 1.5f; //No es necesario modificarlos ya que se calculan por equaciones
    [SerializeField] private float slowSpeed = 1.0f; //No es necesario modificarlos ya que se calculan por equaciones


    [Header("Look Parametres")]
    [SerializeField] private float lookSpeedHorizontal = 2.0f;
    [SerializeField] private float lookSpeedVertical = 2.0f;
    [SerializeField] private float upperLookLimit = 80.0f;
    [SerializeField] private float lowerLookLimit = 80.0f;

    [Header("Jumping Parametres")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Crouch Parametres")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);


    ///Variables
    //Movemnt
    private float velocityMultiplicator;
    private Vector3 moveDirecton;
    private Vector2 currentInput;
    private bool isRuning = false;
    private bool isSlowed = false;

    //Look
    private float rotationX = 0f;

    //Crouch
    private bool isCrouching;
    private bool duringCrouchingAnimation;

    ///References
    private InputHandle _inputHandle;
    private Camera _fpCamera;
    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _fpCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        _inputHandle = InputHandle.Instance;

        runSpeed = walkSpeed * 2;
        crouchSpeed = walkSpeed / 2;
        slowSpeed = walkSpeed / 3;
    }

    private void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if (CanJump)
            {
                HandleJump();
            }
            if (CanCrouch)
            {
                HandleCrouch();
            }

            ApplyFinalMovements();
        }
    }

    private void HandleMovementInput()
    {
        velocityMultiplicator = isSlowed ? slowSpeed : isCrouching ? crouchSpeed : ShouldRun ? runSpeed : walkSpeed;

        isRuning = velocityMultiplicator == runSpeed ? true : false; 

        currentInput = new Vector2(velocityMultiplicator * _inputHandle.MoveInput.y, velocityMultiplicator * _inputHandle.MoveInput.x);

        float moveDirectionY = moveDirecton.y;
        moveDirecton = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirecton.y = moveDirectionY;
    }
    private void HandleMouseLook()
    {
        rotationX -= _inputHandle.LookInput.y * lookSpeedVertical;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        _fpCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, _inputHandle.LookInput.x * lookSpeedHorizontal, 0);
    }
    private void HandleJump()
    {
        if (ShouldJump && !isCrouching)
        {
            moveDirecton.y = jumpForce;
        }
        else if (ShouldJump && isCrouching)
        {
            StartCoroutine(CrouchStand());
        }
    }
    private void HandleCrouch()
    {
        if (ShouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }
    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(_fpCamera.transform.position, Vector3.up, 1f)) 
            yield break;

        duringCrouchingAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = _characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = _characterController.center;

        while (timeElapsed < timeToCrouch)
        {
            _characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            _characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _characterController.height = targetHeight;
        _characterController.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchingAnimation = false;
    }
    private void ApplyFinalMovements()
    {
        if (!_characterController.isGrounded)
        {
            moveDirecton.y -= gravity * Time.deltaTime;
        }

        _characterController.Move(moveDirecton * Time.deltaTime);
    }
}
