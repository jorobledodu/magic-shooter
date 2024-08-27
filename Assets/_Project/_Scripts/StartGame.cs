using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.instance.gameStarted = true;
    }
    private void OnTriggerExit(Collider other)
    {
        Destroy(gameObject);
    }
}
