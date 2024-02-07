using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private PlayerInputActionAsset playerInputActionAsset;
    private FPS_Controller fps_Controller;

    private InputAction pauseAction;

    public GameObject pauseMenu;
    public bool isPaused;

    private void Awake()
    {
        playerInputActionAsset = new PlayerInputActionAsset();
        fps_Controller = new FPS_Controller();

        pauseAction = playerInputActionAsset.Player.Pause;
    }
    private void OnEnable()
    {
        playerInputActionAsset.Enable();

        pauseAction.Enable();
    }
    private void OnDisable()
    {
        //pauseAction.Disable();
    }

    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (pauseAction.triggered)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        fps_Controller.CanLook = false;
        Time.timeScale = 0;
        isPaused = true;

        
    }
}
