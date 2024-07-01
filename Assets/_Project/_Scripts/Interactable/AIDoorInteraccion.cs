using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class AIDoorInteraccion : MonoBehaviour
{
    public Interactable_Door Interactable_Door;

    private void Start()
    {
        Interactable_Door = GetComponentInParent<Interactable_Door>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hOLAAAAAAAAAA");

        if (other.CompareTag("Enemigos"))
        {
            Debug.Log("Enemigo DOOR");
            Interactable_Door.AIOnInteractOpen(other.transform.forward);
        }
    }
}
