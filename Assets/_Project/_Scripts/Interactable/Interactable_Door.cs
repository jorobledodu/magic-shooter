using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Door : Interactable
{
    private bool isOpen = false;
    private bool canBeInteractedWith = true;
    private Animator anim;
    public Vector3 playerTransformDirection;
    public Vector3 doorTransformDirection;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public override void OnFocus()
    {
        Debug.Log("Looking at " + gameObject.name);
    }

    public override void OnInteract()
    {
        if (canBeInteractedWith)
        {
            Debug.Log("Intercted");

            isOpen = !isOpen;

            doorTransformDirection = transform.TransformDirection(Vector3.forward);
            playerTransformDirection = FP_Controller.instance.transform.position - transform.position;
            float dot = Vector3.Dot(doorTransformDirection, playerTransformDirection);

            anim.SetFloat("dot", dot);
            anim.SetBool("isOpen", isOpen);

        }
    }

    public override void OnLoseFocus()
    {
        Debug.Log("Stop Looking at " + gameObject.name);
    }
}
