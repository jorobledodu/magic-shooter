using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTest : MonoBehaviour
{
    //private FP_Controller player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<FP_Controller>();
            if (player != null)
            {
                player.TakeDamage(30);
            }
        }
    }
}
