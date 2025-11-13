using UnityEngine;
using Zenject;

public class Weapon : MonoBehaviour
{
    public event System.Action OnShot;
    public event System.Action<int> OnReload;

    public string WeaponName { get; private set; }
    public int MaxAmmo { get; private set; }
    public int CurrentAmmo { get; private set; }
    public float ReloadTime { get; private set; }
    public float FireRate { get; private set; }
    public float Damage { get; private set; }

    private ParticleSystem muzzleFlash;
    private AudioSource gunSound;

    private float lastFireTime;
    private bool isReloading = false;

    private Camera playerCam;
    public void Initialize(string name, int maxAmmo, float reloadTime, float fireRate, float damage)
    {
        WeaponName = name;
        MaxAmmo = maxAmmo;
        CurrentAmmo = maxAmmo;
        ReloadTime = reloadTime;
        FireRate = fireRate;
        Damage = damage;
    }
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
        CurrentAmmo--;

        OnShot?.Invoke();

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Enemy")))
        {
            Health health = hit.collider.gameObject.GetComponent<Health>();
            if (health != null) health.TakeDamage(Damage);
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
        isReloading = true;

        Invoke(nameof(FinishReload), ReloadTime);
    }
    private void FinishReload()
    {
        isReloading = false;

        CurrentAmmo = MaxAmmo;

        OnReload?.Invoke(MaxAmmo);
    }
    public bool CanReload()
    {
        return !isReloading && CurrentAmmo != MaxAmmo;
    }
    public bool CanFire()
    {
        return CurrentAmmo > 0 && !isReloading && Time.time >= lastFireTime + FireRate;
    }
}
