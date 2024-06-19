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
public enum EstadosDisponibles
{
    Null,
    EnLlamas,
    Electrocutado,
    Mojado
}

public class Magias : MonoBehaviour
{
    public MagiasDisponibles magia;

    private void Update()
    {
        
    }

    public virtual void CambiarMagia(MagiasDisponibles magiaHit)
    {

    }

    public virtual void CambiarEstado()
    {

    }

    public virtual void ComprobarEstado()
    {
    }
}
