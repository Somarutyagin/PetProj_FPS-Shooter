using UnityEngine;
using Zenject;

public class SimplePistol : MonoBehaviour, IWeapon
{
    [Inject] private AmmoViewModel ammoVM;
    private const int MaxAmmoCount = 30;

    private ParticleSystem muzzleFlash;
    private AudioSource gunSound;

    private const float reloadTime = 2f;
    private const float fireRate = 0.3f;
    private const float damage = 25f;

    private float lastFireTime;
    private bool isReloading = false;

    private Camera playerCam;

    private void Awake()
    {
        gunSound = GetComponent<AudioSource>();
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
        if (muzzleFlash != null)
        {
            var main = muzzleFlash.main;
            main.duration = 0.1f;
        }
        playerCam = Camera.main;
    }

    public void Fire()
    {
        if (!CanFire()) return;

        ammoVM.Shoot();

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Enemy")))
        {
            Health health = hit.collider.gameObject.GetComponent<Health>();
            if (health != null) health.TakeDamage(damage);
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        if (gunSound != null)
        {
            gunSound.Play();
        }
        lastFireTime = Time.time;
    }

    public void Reload()
    {
        if (!CanReload()) return;

        isReloading = true;

        Invoke(nameof(FinishReload), reloadTime);
    }
    private void FinishReload()
    {
        isReloading = false;
        ammoVM.Reload(MaxAmmoCount);
    }
    private bool CanReload()
    {
        return !isReloading && ammoVM.CanReload();
    }
    public bool CanFire()
    {
        return ammoVM.CanShoot() && !isReloading && Time.time >= lastFireTime + fireRate;
    }
}
