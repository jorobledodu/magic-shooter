using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CartoonFX.CFXR_Effect;

public class CamaraShake : MonoBehaviour
{
    [Space]
    public CameraShake cameraShake;

    private void Start()
    {

    }

    private void Update()
    {
        if (this.isActiveAndEnabled)
        {
            cameraShake.StartShake();
        }
        else
        {
            cameraShake.StopShake();
        }
    }
}
