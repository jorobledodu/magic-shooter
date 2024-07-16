using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controles : MonoBehaviour
{
    public InputActionReference move;
    public InputActionReference look;
    public InputActionReference jump;
    public InputActionReference run;
    public InputActionReference crouch;
    public InputActionReference interaction;
    public InputActionReference shoot;
    public InputActionReference reload;
    public InputActionReference changeMagic;

    public TextMeshProUGUI textControles;

    private void Awake() { }
    void Start()
    {

    }
}
