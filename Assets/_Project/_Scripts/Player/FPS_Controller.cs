using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class FPS_Controller : MonoBehaviour
{
    public bool CanShoot { get; set; } = true;
    public bool CanReload { get; set; } = true;

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
    public GameObject _prefabHitHole, _prefanHitFlash;
    public VisualEffect _VFXMuzzleFlash;
    public TextMeshProUGUI _bulletsText;

    //Magias
    private bool magia_rayo;
    private bool magia_agua;
    private bool magia_fuego;

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
        //Shoot
        if (readyToShoot && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;

            #region Shoot

            readyToShoot = false;

            //Spread
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);

            //Calculare direction with spread
            Vector3 direction = _fpCamera.transform.forward + new Vector3(x, y, 0);

            //RayCast
            if (Physics.Raycast(_fpCamera.transform.position, direction, out rayHit, range, _whatCanShot))
            {
                if (rayHit.collider.CompareTag("Enemy"))
                {
                    //Graphics  
                    Instantiate(_prefabHitHole, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                    Instantiate(_prefanHitFlash, _fpCamera.transform.position + (rayHit.point - _fpCamera.transform.position) * 0.85f, Quaternion.LookRotation(rayHit.normal));
                }
                else if (rayHit.collider.CompareTag("Floor/Concrete") || rayHit.collider.CompareTag("Floor/Concrete") || rayHit.collider.CompareTag("Floor/Wood"))
                {
                    //Graphics  
                    Instantiate(_prefabHitHole, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                    Instantiate(_prefanHitFlash, _fpCamera.transform.position + (rayHit.point - _fpCamera.transform.position) * 0.85f, Quaternion.LookRotation(rayHit.normal));
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
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
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
}
