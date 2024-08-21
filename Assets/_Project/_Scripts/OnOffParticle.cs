using CartoonFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffParticle : MonoBehaviour
{
    private ParticleSystem thisParticleSystem;
    private CFXR_Effect thisEfectosParticle;

    public bool isOn;
    public GameObject icon;

    // Start is called before the first frame update
    void Start()
    {
        thisParticleSystem = GetComponent<ParticleSystem>();
        thisEfectosParticle = GetComponent<CFXR_Effect>();
        icon = this.transform.GetChild(2).gameObject;

        if (this.CompareTag("Obstaculo/Linterna"))
        {
            StopParticleSystem();
            thisEfectosParticle.enabled = false;
        }
        else if (this.CompareTag("Obstaculo/Fuego"))
        {
            StartParticleSystem();
            thisEfectosParticle.enabled = true;
        }
    }

    public void StartParticleSystem()
    {
        isOn = true;

        thisParticleSystem.Play();
        thisEfectosParticle.enabled = true;

        if (this.CompareTag("Obstaculo/Linterna"))
        {
            icon.SetActive(false);
            GameManager.instance.MasTiempo();
        }
        else if (this.CompareTag("Obstaculo/Fuego"))
        {
            icon.SetActive(true);
            this.GetComponent<Collider>().isTrigger = false;
            return;
        }
    }

    public void StopParticleSystem()
    {
        isOn = false;

        thisParticleSystem.Stop();
        thisEfectosParticle.enabled = false;

        if (this.CompareTag("Obstaculo/Linterna"))
        {
            icon.SetActive(true);
            return;
        }
        else if (this.CompareTag("Obstaculo/Fuego"))
        {
            icon.SetActive(false);
            GameManager.instance.MasTiempo();
            this.GetComponent<Collider>().isTrigger = true;
        }
    }
}
