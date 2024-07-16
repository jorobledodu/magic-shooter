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
    Evaporacion,
    ConCarga,
    Electrocutado,
    Mojado
}

public class Magias : MonoBehaviour
{
    public MagiasDisponibles magia;

    public virtual void CambiarMagia(MagiasDisponibles magiaHit) { }

    public virtual void CambiarEstado() { }

    public virtual void ComprobarEstado() { }

    public virtual void CleanEstado() {  }
}
