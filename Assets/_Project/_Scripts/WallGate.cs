using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGate : MonoBehaviour
{
    public Animator gateAnimator;

    private void OnTriggerEnter(Collider other)
    {
        gateAnimator.SetBool("isOpen", true);
    }

    private void OnTriggerExit(Collider other)
    {
        GetComponent<Collider>().isTrigger = false;
        gateAnimator.SetBool("isOpen", false);
    }
}
