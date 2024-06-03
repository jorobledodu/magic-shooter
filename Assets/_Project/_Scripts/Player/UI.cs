using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject controlesCanvas;
    [SerializeField] private bool gamePause;

    public static UI instance;

    private void OnEnable()
    {
        controlesCanvas.SetActive(true);
    }
    private void OnDisable()
    {
    }
    private void Start()
    {
        instance = this;
    }

    public void onGamePause()
    {
        gamePause = !gamePause;

        controlesCanvas.SetActive(gamePause);

        if (gamePause)
        {
            FP_Controller.instance.CanMove = false;
            FP_Controller.instance.CanLook = false;
            FP_Controller.instance.CanCrouch = false;
            FP_Controller.instance.CanRun = false;
            FP_Controller.instance.CanJump = false;
            FP_Controller.instance.CanInteract = false;
        }
        else
        {
            FP_Controller.instance.CanMove = true;
            FP_Controller.instance.CanLook = true;
            FP_Controller.instance.CanCrouch = true;
            FP_Controller.instance.CanRun = true;
            FP_Controller.instance.CanJump = true;
            FP_Controller.instance.CanInteract = true;
        }
    }
}
