using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Magia", menuName = "ScriptableObject/Magia")]
public class Magias_SO : ScriptableObject
{
    [Header("Parametros")]
    public float fireRate;
    private float fireRateTime = 0;
    public float range;
    public int magazine;

    [Header("Audio")]
    public AudioClip[] sounds;
}
