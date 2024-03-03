using System.Collections;
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
    [SerializeField] private float slowedSpeed = 1.0f; //No es necesario modificarlos ya que se calculan por equaciones


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

    [Header("Headbob Parametres")]
    [SerializeField] private bool canUseHeadbob = true;
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float runBobSpeed = 18f;
    [SerializeField] private float runBobAmount = 0.1f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    [SerializeField] private float slowedBobSpeed = 4f;
    [SerializeField] private float slowedBobAmount = 0.015f;

    [Header("Footsteps Parametres")]
    [SerializeField] private bool useFootsteps = true;
    [SerializeField] private float baseStepsSpeed = 0.5f;
    [SerializeField] private float crouchStepMultipler = 1.5f;
    [SerializeField] private float runStepMultipler = 0.6f;
    [SerializeField] private float slowedStepMultipler = 1.8f;
    [SerializeField] private AudioSource footstepAudioSource = default;
    [SerializeField] private string concreteTag;
    [SerializeField] private AudioClip[] concreteStepsClips = default;
    [SerializeField] private string woodTag;
    [SerializeField] private AudioClip[] woodStepsClips = default;
    [SerializeField] private string wetTag;
    [SerializeField] private AudioClip[] wetStepsClips = default;

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

    //Headbob
    private float defaultYPos = 0;
    private float timer;

    //Footsteps
    private float footstepsTimer = 0;
    private float GetCurrentOffset => (isSlowed ? baseStepsSpeed * slowedStepMultipler : isCrouching ? baseStepsSpeed * crouchStepMultipler : isRuning ? baseStepsSpeed * runStepMultipler : baseStepsSpeed);

    ///References
    private InputHandle _inputHandle;
    private Camera _fpCamera;
    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _fpCamera = Camera.main;

        defaultYPos = _fpCamera.transform.localPosition.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        _inputHandle = InputHandle.Instance;

        runSpeed = walkSpeed * 2;
        crouchSpeed = walkSpeed / 2;
        slowedSpeed = walkSpeed / 3;
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
            if (canUseHeadbob)
            {
                HandleHeadbob();
            }
            if (useFootsteps)
            {
                HandleFootsteps();
            }

            ApplyFinalMovements();
        }
    }

    private void HandleMovementInput()
    {
        velocityMultiplicator = isSlowed ? slowedSpeed : isCrouching ? crouchSpeed : ShouldRun ? runSpeed : walkSpeed;

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
        if (ShouldJump && !Physics.Raycast(_fpCamera.transform.position, Vector3.up, 1f) && !isCrouching)
        {
            moveDirecton.y = jumpForce;
        }
        /// Si le da al espacio estando "crouching" en vez de saltar o no hacer nada, hacer que se levante 
        //else if (ShouldJump && isCrouching)
        //{
        //    StartCoroutine(CrouchStand());
        //    isCrouching = false;
        //}
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
        if (isCrouching && Physics.Raycast(_fpCamera.transform.position, Vector3.up, 1.5f)) 
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
    private void HandleHeadbob()
    {
        if (!_characterController.isGrounded) return;

        if (Mathf.Abs(moveDirecton.x) > 0.1f || Mathf.Abs(moveDirecton.z) > 0.1f)
        {
            timer += Time.deltaTime * (isSlowed ? slowedBobSpeed : isCrouching ? crouchBobSpeed : isRuning ? runBobSpeed : walkBobSpeed);
            _fpCamera.transform.localPosition = new Vector3(_fpCamera.transform.localPosition.x, 
                defaultYPos + Mathf.Sin(timer) * (isSlowed ? slowedBobAmount : isCrouching ? crouchBobAmount : isRuning ? runBobAmount : walkBobAmount), 
                _fpCamera.transform.localPosition.z);
        }
        else
        {
            timer = 0;
            _fpCamera.transform.localPosition = new Vector3(_fpCamera.transform.localPosition.x, 
                Mathf.Lerp(_fpCamera.transform.localPosition.y, defaultYPos, Time.deltaTime * (isSlowed ? slowedBobAmount : isCrouching ? crouchBobAmount : isRuning ? runBobAmount : walkBobAmount)), 
                _fpCamera.transform.localPosition.z);
        }
    }
    private void HandleFootsteps()
    {
        if (!_characterController.isGrounded) return;
        if (currentInput == Vector2.zero) return;

        footstepsTimer -= Time.deltaTime;

        if (footstepsTimer <= 0)
        {
            if (Physics.Raycast(_fpCamera.transform.position, Vector3.down, out RaycastHit hit, 10.0f))
            {
                switch (hit.collider.tag)
                {
                    case "Floor/Concrete":
                        footstepAudioSource.PlayOneShot(concreteStepsClips[Random.Range(0, concreteStepsClips.Length - 1)]);
                        break;
                    case "Floor/Wood":
                        footstepAudioSource.PlayOneShot(woodStepsClips[Random.Range(0, woodStepsClips.Length - 1)]);
                        break;
                    case "Floor/Wet":
                        footstepAudioSource.PlayOneShot(wetStepsClips[Random.Range(0, wetStepsClips.Length - 1)]);
                        Debug.Log("wet steps");
                        break;
                    default: 
                        break;
                }
            }

            footstepsTimer = GetCurrentOffset;
        }
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
