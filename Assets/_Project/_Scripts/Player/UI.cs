using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText = default;

    private void OnEnable()
    {
        FP_Controller.OnDamage += UpdateHealth;
        FP_Controller.OnHeal += UpdateHealth;
    }
    private void OnDisable()
    {
        FP_Controller.OnDamage -= UpdateHealth;
        FP_Controller.OnHeal -= UpdateHealth;
    }
    private void Start()
    {
        UpdateHealth(FP_Controller.instance.maxHealth);
    }

    private void UpdateHealth(float currentHealth)
    {
        healthText.text = currentHealth.ToString("00");
    }
}
