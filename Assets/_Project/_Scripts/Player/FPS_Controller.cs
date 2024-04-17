using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Controller : MonoBehaviour
{

    public bool CanShoot { get; set; } = false;

    public static FPS_Controller instance;

    public bool gun;
    bool shooting, readyToShoot, reloading;

    //References
    public Camera _fpCamera;
    public Transform _attackPoint;
    public LayerMask _whatCanShot;
    public RaycastHit rayHit;

    //Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    private bool magia_rayo;
    private bool magia_agua;
    private bool magia_fuego;

    private void Awake()
    {
        instance = this; 
    }
    private void Update()
    {
        MyInput();

        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    public void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }
    public void Shoot()
    {
        readyToShoot = false;

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculare direction with spread
        Vector3 direction = _fpCamera.transform.forward + new Vector3(x, y, 0);

        //RayCast
        if (Physics.Raycast(_fpCamera.transform.position, direction, out rayHit, range, _whatCanShot))
        {
            Debug.Log(rayHit.collider.name);

            if (rayHit.collider.CompareTag("Enemy")) Debug.Log("Hit enemy " + rayHit.collider.name);
        }

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft < 0)
            Invoke("Shoot", timeBetweenShots);
    }
    private void ResetShot()
    {
        readyToShoot = true;
    }
    public void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
