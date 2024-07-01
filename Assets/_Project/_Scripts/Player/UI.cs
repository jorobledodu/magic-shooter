using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject controlesCanvas;
    [SerializeField] private bool gamePause;




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

        }
    }
}
