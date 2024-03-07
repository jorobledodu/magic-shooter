using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Door : Interactable
{
    public override void OnFocus()
    {
        Debug.Log("Looking at " + gameObject.name);
    }

    public override void OnInteract()
    {
        Debug.Log("Interact with " + gameObject.name);
    }

    public override void OnLoseFocus()
    {
        Debug.Log("Stop Looking at " + gameObject.name);
    }
}
