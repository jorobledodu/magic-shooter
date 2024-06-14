using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MagiasDisponibles
{
    Null,
    Fuego,
    Rayo,
    Agua
}

public class Magias : MonoBehaviour
{
    public MagiasDisponibles magiaActual;
    public MagiasDisponibles magiaAnterior;

    private void Update()
    {
        
    }

    public void CambiarMagia(MagiasDisponibles magiaHit)
    {
        Debug.Log("Magia que entra: " + magiaHit);
        magiaAnterior = magiaActual;
        magiaActual = magiaHit;
    }

    public void ReaccionesMagia()
    {

    }


}
