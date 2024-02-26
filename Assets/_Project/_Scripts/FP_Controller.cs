using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FP_Controller : MonoBehaviour
{
    ///References
    private InputHandle _inputHandle;
    private Camera _fpCamera;
    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _fpCamera = Camera.main;
    }

    private void Start()
    {
        _inputHandle = InputHandle.Instance;
    }
}
