using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FP_Controller : MonoBehaviour
{
    //Condicionales
    public bool CanMove { get; set; } = true;
    public bool CanLook { get; set; } = true;
    public bool CanCrouch { get; set; } = true;
    public bool CanRun { get; set; } = true;
    public bool CanJump { get; set; } = true;
    public bool CanInteract {  get; set; } = true;

    private bool ShouldRun => CanRun && _inputHandle.RunTriggered;
    private bool ShouldCrouch => _inputHandle.CrouchTriggered && !duringCrouchingAnimation && _characterController.isGrounded;
    private bool ShouldJump => _inputHandle.JumpTriggered && _characterController.isGrounded;

    [Header("Movement Parametres")]
    [SerializeField] private float walkSpeed = 3.0f;
    private float runSpeed; //No es necesario modificarlos ya que se calculan por equaciones
    private float crouchSpeed; //No es necesario modificarlos ya que se calculan por equaciones
    private float slowedSpeed; //No es necesario modificarlos ya que se calculan por equaciones

    [Header("Life Parametres")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Gradient healthBarGradient;
    [SerializeField] private float maxHealth = 100.0f;
    [SerializeField] private float timeBeforeRegenHealth = 3.0f;
    [SerializeField] private float healthValueIncrement = 1.0f;
    [SerializeField] private float healthTimeIncrement = 0.1f;

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
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultipler = 1.5f;
    [SerializeField] private float runStepMultipler = 0.6f;
    [SerializeField] private float slowedStepMultipler = 1.8f;
    [SerializeField] private AudioSource _SFXAudioSource = default;
    [SerializeField] private AudioClip[] concreteStepsClips = default;
    [SerializeField] private AudioClip[] woodStepsClips = default;
    [SerializeField] private AudioClip[] wetStepsClips = default;
    [SerializeField] private AudioClip[] dirtStepsClips = default;

    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;
    [SerializeField] private TextMeshProUGUI interactuarText;
    [SerializeField] private Image interactionCroshairImage;
    private Interactable currentInteractable;

    ///Variables
    //Movemnt
    private float velocityMultiplicator;
    private Vector3 moveDirecton;
    private Vector2 currentInput;
    public bool isMoving = false;
    public bool isRuning = false;
    public bool isSlowed = false;

    //Health
    public float currentHealth;
    private Coroutine regeneratingHealth;

    //Look
    private float rotationX = 0f;

    //Crouch
    private bool isCrouching;
    private bool duringCrouchingAnimation;

    //Headbob
    private float defaultYPos = 0;
    private float timer;

    //Footstep
    private float footstepTimer = 0f;
    private float GetCurrentOffset => isSlowed ? baseStepSpeed * slowedStepMultipler : isCrouching ? baseStepSpeed * crouchStepMultipler : isRuning ? baseStepSpeed * runStepMultipler : baseStepSpeed;

    ///References
    private Player_InputHandle _inputHandle;
    private Camera _fpCamera;
    private CharacterController _characterController;
    public static FP_Controller instance;   

    private void Awake()
    {
        instance = this; 

        _characterController = GetComponent<CharacterController>();
        _fpCamera = Camera.main;

        defaultYPos = _fpCamera.transform.localPosition.y;
        currentHealth = maxHealth;
    }

    private void Start()
    {
        _inputHandle = Player_InputHandle.instance;

        runSpeed = walkSpeed * 1.5f;
        crouchSpeed = walkSpeed / 1.5f;
        slowedSpeed = walkSpeed / 2.5f;

        interactuarText.text = "";

        interactionCroshairImage.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();

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
        if (CanLook)
        {
            HandleMouseLook();
        }
        if (CanJump)
        {
            HandleJump();
        }
        if (CanCrouch)
        {
            HandleCrouch();
        }
        if (CanInteract)
        {
            HandleInteractionCheck();
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

        isMoving = currentInput.x != 0 || currentInput.y != 0;
    }
    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0.0f)
        {
            Death();
            StopCoroutine(regeneratingHealth);
        }
        else if (regeneratingHealth != null)
        {
            StopCoroutine(regeneratingHealth);
        }
        regeneratingHealth = StartCoroutine(RegenerateHealth());

        UpdateHealthBar();
    }
    private void UpdateHealthBar()
    {
        float targetFillAmount = currentHealth / maxHealth;
        healthBarFill.fillAmount = targetFillAmount;
        healthBarFill.color = healthBarGradient.Evaluate(targetFillAmount);
    }
    private void Death()
    {
        currentHealth = 0;

    }
    private IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(timeBeforeRegenHealth);
        WaitForSeconds timeToWait = new WaitForSeconds(healthTimeIncrement);

        while (currentHealth < maxHealth)
        {
            currentHealth += healthValueIncrement;

            if (currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
            }

            yield return timeToWait;

            UpdateHealthBar();
        }

        regeneratingHealth = null;
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
        if (ShouldJump && !Physics.Raycast(_fpCamera.transform.position, Vector3.up, 0.5f) && !isCrouching)
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
        }
    }
    private void HandleFootsteps()
    {
        if (!_characterController.isGrounded) return;
        if (!isMoving) return;

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3, floorLayer, QueryTriggerInteraction.Collide))
            {
                switch (hit.collider.tag)
                {
                    case "Floor/Concrete":
                        _SFXAudioSource.PlayOneShot(concreteStepsClips[UnityEngine.Random.Range(0, concreteStepsClips.Length - 1)]);
                        break;
                    case "Floor/Wood":
                        _SFXAudioSource.PlayOneShot(woodStepsClips[UnityEngine.Random.Range(0, woodStepsClips.Length - 1)]);
                        break;
                    case "Floor/Wet":
                        _SFXAudioSource.PlayOneShot(wetStepsClips[UnityEngine.Random.Range(0, wetStepsClips.Length - 1)]);
                        break;
                    case "Floor/Dirt":
                        _SFXAudioSource.PlayOneShot(dirtStepsClips[UnityEngine.Random.Range(0, dirtStepsClips.Length - 1)]);
                        break;
                    default:
                        _SFXAudioSource.PlayOneShot(dirtStepsClips[UnityEngine.Random.Range(0, dirtStepsClips.Length - 1)]);
                        break;
                }
            }

            footstepTimer = GetCurrentOffset;
        }
    }
    private void HandleInteractionCheck()
    {
        //Condicion if: raycast hace hit 
        if (Physics.Raycast(_fpCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            //Condicion if: si el objeto contra el que colisiona el raycast esta en la layer 8 (interactable layer) entra en estado de focus (estamos mirando un objeto interactuable)
            //Explicacion parte InstanceID: esto sirve para que si el objeto que estas mirando no es el mismo que hay almacenado en el currentInteractable lo sustituya por el Interactable del que estas mirando
            if (hit.collider.gameObject.layer == 8 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.gameObject.GetInstanceID()))
            {
                //Add: cuando mires un objeto interactuable, en la mirilla saldra un circulo o habra una forma de feedback para el jugador

                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable)
                {
                    currentInteractable.OnFocus();
                    interactuarText.text = "F Interactuar";
                    interactionCroshairImage.gameObject.SetActive(true);
                }
            }
            //Condicion else if: si estabamos mirando un objeto interactuable y el raycast ya no hace hit y la layer del objeto no coincide con la layer de interaccion, entramos en estado on lose focus (hemos dejado de mirar un objeto interactuable)
            else if (currentInteractable && hit.collider.gameObject.layer != 8)
            {
                //Add: pierde el feedback de que estas mirando algo interactuable

                currentInteractable.OnLoseFocus();
                currentInteractable = null;
                interactuarText.text = "";
                interactionCroshairImage.gameObject.SetActive(false);
            }
        }
        //Condicion else if: si estabamos mirando un objeto interactuable y el raycast ya no hace hit, entramos en estado on lose focus (hemos dejado de mirar un objeto interactuable)
        else if (currentInteractable)
        {
            //Add: pierde el feedback de que estas mirando algo interactuable

            currentInteractable.OnLoseFocus();
            currentInteractable = null;
            interactuarText.text = "";
            interactionCroshairImage.gameObject.SetActive(false);
        }
    }
    public void HandleInteractionInput()
    {
        //Condicion if: si interaction trigger y estamos mirando un objeto interactuable
        if (currentInteractable != null && Physics.Raycast(_fpCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
        {
            currentInteractable.OnInteract();
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
