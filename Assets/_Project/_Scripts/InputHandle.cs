using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandle : MonoBehaviour
{
    public static InputHandle Instance;
    private PlayerInputActionAsset playerInputActionAsset;

    #region Player InputAction Reference
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    #endregion

    #region UI InputAction Reference
    private InputAction pauseAction;
    #endregion

    public Vector2 MoveInput {  get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool SprintTriggered { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool CrouchTriggered { get; private set; }


    private void Awake()
    {
        #region Instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion

        playerInputActionAsset = new PlayerInputActionAsset();

        moveAction = playerInputActionAsset.Player.Move;
        lookAction = playerInputActionAsset.Player.Look;
        jumpAction = playerInputActionAsset.Player.Jump;
        sprintAction = playerInputActionAsset.Player.Sprint;
        crouchAction = playerInputActionAsset.Player.Crouch;

        RegisterInputs();
    }
    private void OnEnable()
    {
        playerInputActionAsset.Enable();

        moveAction.Enable(); 
        lookAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
        crouchAction.Enable();
    }
    private void OnDisable()
    {
        playerInputActionAsset.Disable();

        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
        crouchAction.Disable();
    }

    private void RegisterInputs()
    {
        moveAction.performed += onMove;
        moveAction.canceled += onMoveCanceled;

        lookAction.performed += onLook;
        lookAction.canceled += onLookCanceled;

        jumpAction.performed += onJump;
        jumpAction.canceled += onJumpCanceled;

        sprintAction.performed += onSprint;
        sprintAction.canceled += onSprintCanceled;

        crouchAction.performed += onCrouch;
        crouchAction.canceled += onCrouchCanceled;
    }

    private void onMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }

    private void onMoveCanceled(InputAction.CallbackContext ctx)
    {
        MoveInput = Vector2.zero;
    }

    private void onLook(InputAction.CallbackContext ctx)
    {
        LookInput = ctx.ReadValue<Vector2>();
    }

    private void onLookCanceled(InputAction.CallbackContext ctx)
    {
        LookInput = Vector2.zero;
    }

    private void onJump(InputAction.CallbackContext ctx)
    {
        JumpTriggered = true;
    }

    private void onJumpCanceled(InputAction.CallbackContext ctx)
    {
        JumpTriggered = false;
    }

    private void onSprint(InputAction.CallbackContext ctx)
    {
        SprintTriggered = true;
    }

    private void onSprintCanceled(InputAction.CallbackContext ctx)
    {
        SprintTriggered = false;
    }

    private void onCrouch(InputAction.CallbackContext ctx)
    {
        CrouchTriggered = CrouchTriggered ? false : true;
    }

    private void onCrouchCanceled(InputAction.CallbackContext ctx)
    {
        //CrouchTriggered = false;
    }
}
