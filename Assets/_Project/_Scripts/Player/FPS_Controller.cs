using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.VFX;

[Serializable]
public class ListaMagias
{
    [SerializeField] private bool activa;
    [SerializeField] private MagiasDisponibles magia;
    [SerializeField] private GameObject VFX;

    public bool IsActiva => activa;
    public MagiasDisponibles TipoMagia => magia;

    public void Activar()
    {
        activa = true;
        if (VFX != null)
        {
            VFX.SetActive(true);
        }
    }

    public void Desactivar()
    {
        activa = false;
        if (VFX != null)
        {
            VFX.SetActive(false);
        }
    }
}
public class FPS_Controller : MonoBehaviour
{
    public static FPS_Controller instance { get; private set; }

    public bool CanShoot { get; set; } = true;
    public bool CanReload { get; set; } = true;
    public bool CanChangeMagic { get; set; } = true;

    //References
    public Camera _fpCamera;
    
    public LayerMask _whatCanShot;
    public RaycastHit rayHit;
    public Animator _animator;
    public AudioSource _SFXAudioSource;

    //Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    int bulletsLeft, bulletsShot;
    bool readyToShoot, reloading;
    [SerializeField] private AudioClip _pistolShotAudio;
    public GameObject _shotHitTest;

    //Graphics
    public GameObject _prefabHitHole, _prefanHitFlash, _prefabHit;
    public VisualEffect _VFXMuzzleFlash;
    public TextMeshProUGUI _bulletsText;

    //Magias
    public ListaMagias[] listaMagias;
    public bool magia_rayo;
    public bool magia_agua;
    public bool magia_fuego;
    public MagiasDisponibles magiaActiva;

    private void Awake()
    {
        instance = this;

        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    private void Update()
    {   
        //Set Text
        _bulletsText.SetText(bulletsLeft + " / " + magazineSize);
    }

    public void Shoot()
    {
        // Encontrar el índice del elemento activo
        int currentIndex = -1;
        for (int i = 0; i < listaMagias.Length; i++)
        {
            if (listaMagias[i].IsActiva)
            {
                currentIndex = i;
                break;
            }
        }

        //Shoot
        if (readyToShoot && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;

            #region Shoot

            readyToShoot = false;

            //Spread
            float x = UnityEngine.Random.Range(-spread, spread);
            float y = UnityEngine.Random.Range(-spread, spread);

            //Calculare direction with spread
            Vector3 direction = _fpCamera.transform.forward + new Vector3(x, y, 0);

            //RayCast
            if (Physics.Raycast(_fpCamera.transform.position, direction, out rayHit, range, _whatCanShot))
            {
                if (rayHit.collider.CompareTag("Enemigos"))
                {
                    Debug.Log("Enemigo golpeado");

                    //Coger script AIUnit
                    AIUnit aiUnitEnemigo = rayHit.transform.GetComponentInParent<AIUnit>();
                    aiUnitEnemigo.RecibirDaño(damage);

                    //Graphics  
                    Instantiate(_prefabHit, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                    Instantiate(_prefanHitFlash, _fpCamera.transform.position + (rayHit.point - _fpCamera.transform.position) * 0.85f, Quaternion.LookRotation(rayHit.normal));

                    //Comprobar si el objetivo tiene el script "Magias"
                    if (rayHit.transform.TryGetComponent(out Magias magiasHit))
                    {
                        magiasHit.CambiarMagia(listaMagias[currentIndex].TipoMagia);
                        magiasHit.CambiarEstado();
                        magiasHit.CleanEstado();
                    }
                    else if (rayHit.transform.parent.root.TryGetComponent(out Magias magiasHitP))
                    {
                        magiasHitP.CambiarMagia(listaMagias[currentIndex].TipoMagia);
                        magiasHitP.CambiarEstado();
                        magiasHitP.CleanEstado();
                    }
                }
                else if (rayHit.collider.CompareTag("NPC"))
                {
                    //Graphics  
                    Instantiate(_prefabHit, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                    Instantiate(_prefanHitFlash, _fpCamera.transform.position + (rayHit.point - _fpCamera.transform.position) * 0.85f, Quaternion.LookRotation(rayHit.normal));
                }
                else if (rayHit.collider.CompareTag("Obstaculo/Fuego"))
                {
                    //Graphics  
                    Instantiate(_prefabHit, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                    Instantiate(_prefanHitFlash, _fpCamera.transform.position + (rayHit.point - _fpCamera.transform.position) * 0.85f, Quaternion.LookRotation(rayHit.normal));

                    if (listaMagias[currentIndex].TipoMagia == MagiasDisponibles.Agua)
                    {
                        rayHit.transform.GetComponent<OnOffParticle>().StopParticleSystem();
                    }
                }
                else if (rayHit.collider.CompareTag("Obstaculo/Linterna"))
                {
                    //Graphics  
                    Instantiate(_prefabHit, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                    Instantiate(_prefanHitFlash, _fpCamera.transform.position + (rayHit.point - _fpCamera.transform.position) * 0.85f, Quaternion.LookRotation(rayHit.normal));

                    if (listaMagias[currentIndex].TipoMagia == MagiasDisponibles.Fuego)
                    {
                        rayHit.transform.GetComponent<OnOffParticle>().StartParticleSystem();
                    }
                }
                else
                {
                    //Graphics  
                    Instantiate(_prefabHitHole, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                    Instantiate(_prefanHitFlash, _fpCamera.transform.position + (rayHit.point - _fpCamera.transform.position) * 0.85f, Quaternion.LookRotation(rayHit.normal));
                }
            }

            _shotHitTest.transform.position = rayHit.point;

            //Sound
            _SFXAudioSource.PlayOneShot(_pistolShotAudio);

            //Call animation
            _animator.SetBool("Shoot", true);

            bulletsLeft--;
            bulletsShot--;

            Invoke("ResetShot", timeBetweenShooting);

            if (bulletsShot > 0 && bulletsLeft < 0)
                Invoke("Shoot", timeBetweenShots);
            #endregion
        }
    }
    private void ResetShot()
    {
        readyToShoot = true;
    }
    public void Reload()
    {
        if (bulletsLeft < magazineSize && !reloading)
        {
            reloading = true;
            _animator.SetBool("Reload", true);
            Invoke("ReloadFinished", reloadTime);
        }
    }
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        _animator.SetBool("Reload", false);
        reloading = false;
    }

    public void ChangeMagic(float value)
    {
        // Encontrar el índice del elemento activo
        int currentIndex = -1;
        for (int i = 0; i < listaMagias.Length; i++)
        {
            if (listaMagias[i].IsActiva)
            {
                currentIndex = i;
                break;
            }
        }

        // Si no hay un elemento activo, no hacer nada
        if (currentIndex == -1)
        {
            return;
        }

        // Desactivar el elemento actual
        listaMagias[currentIndex].Desactivar();

        // Calcular el nuevo índice basado en value
        int newIndex;
        if (value > 0)
        {
            newIndex = (currentIndex - 1 + listaMagias.Length) % listaMagias.Length;
        }
        else if (value < 0)
        {
            newIndex = (currentIndex + 1) % listaMagias.Length;
        }
        else
        {
            // Si value es 0, no hacer nada
            listaMagias[currentIndex].Activar();
            return;
        }

        // Activar el nuevo elemento
        listaMagias[newIndex].Activar();
    }
}
