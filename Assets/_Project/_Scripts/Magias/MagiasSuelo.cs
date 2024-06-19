using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagiasSuelo : Magias
{
    public EstadosDisponibles estado;

    public override void CambiarMagia(MagiasDisponibles magiaHit)
    {
        magia = magiaHit;
    }

    public override void CambiarEstado()
    {
        if (magia == MagiasDisponibles.Rayo && estado == EstadosDisponibles.Mojado)
        {
            Debug.Log("Electrocutar");
            estado = EstadosDisponibles.Electrocutado;
        }
    }

    //IEnumerator ElectrocutadoAccion()
    //{
    //    return 
    //}
}
