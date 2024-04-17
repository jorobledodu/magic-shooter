using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText = default;
    [SerializeField] private GameObject controlesCanvas;
    [SerializeField] private bool gamePause;

    public static UI instance;

    private void OnEnable()
    {
        FP_Controller.OnDamage += UpdateHealth;
        FP_Controller.OnHeal += UpdateHealth;

        controlesCanvas.SetActive(true);
    }
    private void OnDisable()
    {
        FP_Controller.OnDamage -= UpdateHealth;
        FP_Controller.OnHeal -= UpdateHealth;
    }
    private void Start()
    {
        instance = this;

        UpdateHealth(FP_Controller.instance.maxHealth);
    }

    private void UpdateHealth(float currentHealth)
    {
        healthText.text = currentHealth.ToString("00");
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
