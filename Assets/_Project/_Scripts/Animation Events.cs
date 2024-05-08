using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AnimationEvents : MonoBehaviour
{
    private Animator _animator;
    public VisualEffect _muzzleFlash;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void ShotFalse()
    {
        _animator.SetBool("Shoot", false);
    }

    public void MuzzleFlashOn()
    {
        _muzzleFlash.Play();
    }

    public void AIAttackFalse()
    {
        _animator.SetBool("isAttaking", false);
    }
}
