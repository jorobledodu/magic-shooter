using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_NPC : Interactable
{
    public override void OnFocus()
    {
        Debug.Log("Looking at " + gameObject.name);
    }

    public override void OnInteract()
    {
        Debug.Log("Interacting with " + gameObject.name);
    }

    public override void OnLoseFocus()
    {
        Debug.Log("Stop Looking at " + gameObject.name);
    }
}
