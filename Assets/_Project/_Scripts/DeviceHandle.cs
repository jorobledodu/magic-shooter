using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceHandle : MonoBehaviour
{
    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PrintDevices();
    }

    void PrintDevices()
    {
        //foreach (var device in InputSystem.devices)
        //{
        //    if (device.enabled)
        //    {
        //        Debug.Log("Active Device: " + device.name);
        //    }
        //}

        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
        {
            Debug.Log("Active Device: Gamepad");
        }
    }
}
