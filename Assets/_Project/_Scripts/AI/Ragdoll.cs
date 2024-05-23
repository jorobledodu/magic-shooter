using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rigidBodies;
    Animator animator;

    private void Start()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();

        DesactivarRagdoll();
    }

    public void DesactivarRagdoll()
    {
        foreach (var rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = true;
        }

        animator.enabled = true;
    }

    public void ActivarRagdoll()
    {
        foreach (var rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = false;
        }

        animator.enabled = false;
    }
}
