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
    private InputAction changeMagicNullAction;
    private InputAction changeMagic1Action;
    private InputAction changeMagic2Action;
    private InputAction changeMagic3Action;
    #endregion

    public Vector2 MoveInput {  get; private set; }
    public Vector2 LookInput;
    public bool RunTriggered { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool CrouchTriggered { get; private set; }

    private void Awake()
    {
        #region Instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
        changeMagicNullAction = playerInputActionAsset.Player.ChangeMagicNull;
        changeMagic1Action = playerInputActionAsset.Player.ChangeMagic1;
        changeMagic2Action = playerInputActionAsset.Player.ChangeMagic2;
        changeMagic3Action = playerInputActionAsset.Player.ChangeMagic3;
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
        changeMagicNullAction.Enable();
        changeMagic1Action.Enable();
        changeMagic2Action.Enable();
        changeMagic3Action.Enable();

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
        changeMagicNullAction.Disable();
        changeMagic1Action.Disable();
        changeMagic2Action.Disable();
        changeMagic3Action.Disable();

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

        changeMagicNullAction.performed += onChangeMagicNull;
        changeMagic1Action.performed += onChangeMagic1;
        changeMagic2Action.performed += onChangeMagic2;
        changeMagic3Action.performed += onChangeMagic3;

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

        changeMagicNullAction.performed -= onChangeMagicNull;
        changeMagic1Action.performed -= onChangeMagic1;
        changeMagic2Action.performed -= onChangeMagic2;
        changeMagic3Action.performed -= onChangeMagic3;

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

    private void onInteraction(InputAction.CallbackContext context)
    {
        if (FP_Controller.instance.CanInteract)
        {
            FP_Controller.instance.HandleInteractionInput();
        }
    }
    private void onInteractionCanceled(InputAction.CallbackContext context)
    {
    }

    private void onShoot(InputAction.CallbackContext context)
    {
        if (FPS_Controller.instance.CanShoot)
        {
            FPS_Controller.instance.Shoot();
        }
    }

    private void onReload(InputAction.CallbackContext context)
    {
        if (FPS_Controller.instance.CanReload)
        {
            FPS_Controller.instance.Reload();
        }
    }

    private void onChangeMagicNull(InputAction.CallbackContext context)
    {
        if (FPS_Controller.instance.CanChangeMagic)
        {
            FPS_Controller.instance.ChangeMagicNull();
        }
    }
    private void onChangeMagic1(InputAction.CallbackContext context)
    {
        if (FPS_Controller.instance.CanChangeMagic)
        {
            FPS_Controller.instance.ChangeMagic1();
        }
    }
    private void onChangeMagic2(InputAction.CallbackContext context)
    {
        if (FPS_Controller.instance.CanChangeMagic)
        {
            FPS_Controller.instance.ChangeMagic2();
        }
    }
    private void onChangeMagic3(InputAction.CallbackContext context)
    {
        if (FPS_Controller.instance.CanChangeMagic)
        {
            FPS_Controller.instance.ChangeMagic3();
        }
    }
}
