using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.VFX;

public class FPS_Controller : MonoBehaviour
{
    public bool CanShoot { get; set; } = true;
    public bool CanReload { get; set; } = true;
    public bool CanChangeMagic { get; set; } = true;

    public static FPS_Controller instance;

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
    public bool magia_rayo;
    public bool magia_agua;
    public bool magia_fuego;
    public Transform magiasHandle;
    public MagiasDisponibles magiaActiva;

    private void Awake()
    {
        instance = this;

        bulletsLeft = magazineSize;
        readyToShoot = true;

        ChangeMagicNull();
    }
    private void Update()
    {   
        //Set Text
        _bulletsText.SetText(bulletsLeft + " / " + magazineSize);
    }

    public void Shoot()
    {
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
                }
                else
                {
                    //Graphics  
                    Instantiate(_prefabHitHole, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                    Instantiate(_prefanHitFlash, _fpCamera.transform.position + (rayHit.point - _fpCamera.transform.position) * 0.85f, Quaternion.LookRotation(rayHit.normal));
                }

                //Comprobar si el objetivo tiene el script "Magias"
                if (rayHit.transform.TryGetComponent(out Magias magiasHit))
                {
                    magiasHit.CambiarMagia(magiaActiva);
                    magiasHit.CambiarEstado();
                    magiasHit.ComprobarEstado();
                }
                else if (rayHit.transform.parent.root.TryGetComponent(out Magias magiasHitP))
                {
                    magiasHitP.CambiarMagia(magiaActiva);
                    magiasHitP.CambiarEstado();
                    magiasHitP.ComprobarEstado();
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

    public void ChangeMagicNull()
    {
        magiasHandle.GetChild(0).gameObject.SetActive(false);
        magiasHandle.GetChild(1).gameObject.SetActive(false);
        magiasHandle.GetChild(2).gameObject.SetActive(false);

        magiaActiva = MagiasDisponibles.Null;
    }
    public void ChangeMagic1()
    {
        magiasHandle.GetChild(0).gameObject.SetActive(true);
        magiasHandle.GetChild(1).gameObject.SetActive(false);
        magiasHandle.GetChild(2).gameObject.SetActive(false);

        magiaActiva = magiasHandle.GetChild(0).GetComponent<Magias>().magia;
    }
    public void ChangeMagic2()
    {
        magiasHandle.GetChild(0).gameObject.SetActive(false);
        magiasHandle.GetChild(1).gameObject.SetActive(true);
        magiasHandle.GetChild(2).gameObject.SetActive(false);

        magiaActiva = magiasHandle.GetChild(1).GetComponent<Magias>().magia;
    }
    public void ChangeMagic3()
    {
        magiasHandle.GetChild(0).gameObject.SetActive(false);
        magiasHandle.GetChild(1).gameObject.SetActive(false);
        magiasHandle.GetChild(2).gameObject.SetActive(true);

        magiaActiva = magiasHandle.GetChild(2).GetComponent<Magias>().magia;
    }
}
