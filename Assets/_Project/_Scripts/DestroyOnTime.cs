using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
    public int _lifeTime;

    void Start()
    {
        Destroy(this.gameObject, _lifeTime);
    }
}
