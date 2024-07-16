using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(0)]
public class Player_InputHandle : MonoBehaviour
{
    public static Player_InputHandle instance;
    private PlayerInputActionAsset playerInputActionAsset;

    #region Player InputAction Reference
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction runAction;
    private InputAction crouchAction;
    private InputAction interactionAction;
    private InputAction shootAction;
    private InputAction reloadAction;
    private InputAction changeMagicAction;
    #endregion

    public Vector2 MoveInput {  get; private set; }
    public Vector2 LookInput {  get; private set; }
    public bool RunTriggered { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool CrouchTriggered { get; private set; }

    private void Awake()
    {
        #region Instance
        instance = this;
        #endregion

        playerInputActionAsset = new PlayerInputActionAsset();

        #region Player
        moveAction = playerInputActionAsset.Player.Move;
        lookAction = playerInputActionAsset.Player.Look;
        jumpAction = playerInputActionAsset.Player.Jump;
        runAction = playerInputActionAsset.Player.Run;
        crouchAction = playerInputActionAsset.Player.Crouch;
        interactionAction = playerInputActionAsset.Player.Interaction;
        shootAction = playerInputActionAsset.Player.Shoot;
        reloadAction = playerInputActionAsset.Player.Reload;
        changeMagicAction = playerInputActionAsset.Player.ChangeMagic;
        #endregion
    }
    private void OnEnable()
    {
        playerInputActionAsset.Enable();

        moveAction.Enable(); 
        lookAction.Enable();
        jumpAction.Enable();
        runAction.Enable();
        crouchAction.Enable();
        interactionAction.Enable();
        shootAction.Enable();
        reloadAction.Enable();
        changeMagicAction.Enable();

        SubscribeInputs();
    }
    private void OnDisable()
    {
        playerInputActionAsset.Disable();

        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        runAction.Disable();
        crouchAction.Disable();
        interactionAction.Disable();
        shootAction.Disable();
        reloadAction.Disable();
        changeMagicAction.Disable();

        UnsubscribeInputs();
    }

    private void SubscribeInputs()
    {
        moveAction.performed += onMove;
        moveAction.canceled += onMoveCanceled;

        lookAction.performed += onLook;
        lookAction.canceled += onLookCanceled;

        jumpAction.performed += onJump;
        jumpAction.canceled += onJumpCanceled;

        runAction.performed += onRun;
        runAction.canceled += onRunCanceled;

        crouchAction.performed += onCrouch;
        crouchAction.canceled += onCrouchCanceled;

        interactionAction.performed += onInteraction;

        shootAction.performed += onShoot;

        reloadAction.performed += onReload;

        changeMagicAction.performed += onChangeMagic;
    }

    private void UnsubscribeInputs()
    {
        moveAction.performed -= onMove;
        moveAction.canceled -= onMoveCanceled;

        lookAction.performed -= onLook;
        lookAction.canceled -= onLookCanceled;

        jumpAction.performed -= onJump;
        jumpAction.canceled -= onJumpCanceled;
            
        runAction.performed -= onRun;
        runAction.canceled -= onRunCanceled;

        crouchAction.performed -= onCrouch;
        crouchAction.canceled -= onCrouchCanceled;

        interactionAction.performed -= onInteraction;

        shootAction.performed -= onShoot;

        reloadAction.performed -= onReload;

        changeMagicAction.performed -= onChangeMagic;
    }

    private void onMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }

    private void onMoveCanceled(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }

    private void onLook(InputAction.CallbackContext ctx)
    {
        LookInput = ctx.ReadValue<Vector2>();
    }

    private void onLookCanceled(InputAction.CallbackContext ctx)
    {
        LookInput = ctx.ReadValue<Vector2>();
    }

    private void onJump(InputAction.CallbackContext ctx)
    {
        JumpTriggered = true;
    }

    private void onJumpCanceled(InputAction.CallbackContext ctx)
    {
        JumpTriggered = false;
    }

    private void onRun(InputAction.CallbackContext ctx)
    {
        RunTriggered = true;
    }

    private void onRunCanceled(InputAction.CallbackContext ctx)
    {
        RunTriggered = false;
    }

    private void onCrouch(InputAction.CallbackContext ctx)
    {
        CrouchTriggered = true;
    }

    private void onCrouchCanceled(InputAction.CallbackContext ctx)
    {
        CrouchTriggered = false;
    }

    private void onInteraction(InputAction.CallbackContext ctx)
    {
        if (FP_Controller.instance.CanInteract)
        {
            FP_Controller.instance.HandleInteractionInput();
        }
    }

    private void onShoot(InputAction.CallbackContext ctx)
    {
        if (FPS_Controller.instance.CanShoot)
        {
            FPS_Controller.instance.Shoot();
        }
    }

    private void onReload(InputAction.CallbackContext ctx)
    {
        if (FPS_Controller.instance.CanReload)
        {
            FPS_Controller.instance.Reload();
        }
    }
    private void onChangeMagic(InputAction.CallbackContext ctx)
    {
        float valor = ctx.ReadValue<float>();

        if (FPS_Controller.instance.CanChangeMagic)
        {
            FPS_Controller.instance.ChangeMagic(valor);
        }
    }
}
