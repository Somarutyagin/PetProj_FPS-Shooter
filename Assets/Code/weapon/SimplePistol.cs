using UnityEngine;

public class SimplePistol : MonoBehaviour, IWeapon
{
    private ParticleSystem muzzleFlash;
    private AudioSource gunSound;
    public readonly int maxAmmo = 30;

    private const float reloadTime = 2f;
    private const float fireRate = 0.3f;
    private const float damage = 25f;

    private int currentAmmo;
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
        currentAmmo = maxAmmo;
        playerCam = Camera.main;
    }

    public void Fire()
    {
        if (!CanFire()) return;

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Enemy")))
        {
            Health health = hit.collider.gameObject.GetComponent<Health>();
            if (health != null) health.TakeDamage(damage);
        }

        currentAmmo--;
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
        if (isReloading) return;
        isReloading = true;

        Invoke(nameof(FinishReload), reloadTime);
    }

    private void FinishReload()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    public bool CanFire()
    {
        return currentAmmo > 0 && !isReloading && Time.time >= lastFireTime + fireRate;
    }

    public int GetAmmo() => currentAmmo;
    public int GetMaxAmmoInfo() => maxAmmo;
    public void SetAmmo(int ammo) => currentAmmo = ammo;
}
