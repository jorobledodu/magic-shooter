using System;
using UnityEngine;

[Serializable]
public class Dialogo
{
    [SerializeField]
    private string persona;
    [SerializeField]
    private string contenido;

    public string Persona => persona;
    public string Contenido => contenido;
}
