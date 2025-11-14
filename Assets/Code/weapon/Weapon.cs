using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;

public class Weapon : MonoBehaviour
{
    private const float baseRecoil = 0.05f;
    private const float recoilIncreasePerShot = 0.008f;
    private const float stabilizationSpeed = 5f;
    private const float stabilizationDelay = 0.3f;
    private const float maxRecoil = 0.2f;

    private const float baseSpread = 0.005f;
    private const float spreadIncreasePerShot = 0.004f;
    private const float maxSpread = 0.1f;

    private float currentRecoil = 0f;
    private float targetRecoil = 0f;
    private float currentSpread = 0f;
    private int shotCounter = 0;

    private Animator anim;
    private AnimationClip reloadClip;
    private GameObject bulletTracerPrefab;
    private GameObject bulletHolePrefab;
    private Transform muzzlePoint;
    private ParticleSystem muzzleFlash;
    private AudioSource gunSound;
    [SerializeField] private AudioClip impactSound;
    private float tracerDuration = 0.15f;
    private const float impactEffectDuration = 2f;

    private Transform bulletTracerContainer;
    private Transform bulletHoleContainer;
    private const int poolSize = 50;
    private ObjectPool<BulletTracer> tracerPool;
    private ObjectPool<Effect> impactPool;

    public event System.Action OnShot;
    public event System.Action<int> OnReload;

    public string WeaponName { get; private set; }
    public int MaxAmmo { get; private set; }
    public int CurrentAmmo { get; private set; }
    public float ReloadTime { get; private set; }
    public float FireRate { get; private set; }
    public float Damage { get; private set; }

    private float lastFireTime;
    private bool isReloading = false;

    private Camera playerCam;
    private Vector3 baseCameraEuler;
    private LayerMask enemyLayer;

    public void Initialize(string name, int maxAmmo, float reloadTime, float fireRate, float damage, GameObject _bulletTracerPrefab, GameObject _bulletHolePrefab)
    {
        WeaponName = name;
        MaxAmmo = maxAmmo;
        CurrentAmmo = maxAmmo;
        ReloadTime = reloadTime;
        FireRate = fireRate;
        Damage = damage;
        bulletTracerPrefab = _bulletTracerPrefab;
        bulletHolePrefab = _bulletHolePrefab;

        muzzlePoint = transform.GetChild(transform.childCount - 1);
        gunSound = GetComponent<AudioSource>();
        muzzleFlash = GetComponentInChildren<ParticleSystem>();

        if (TryGetComponent(out Animator _anim))
        {
            anim = _anim;
            reloadClip = anim.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == "reload");
        }

        bulletTracerContainer = GameObject.Find("Tracer Transform Pool").transform;
        bulletHoleContainer = GameObject.Find("Bullet Hole Transform Pool").transform;

        if (muzzleFlash != null)
        {
            var main = muzzleFlash.main;
            main.duration = 0.1f;
        }
        playerCam = Camera.main;
        if (playerCam != null)
        {
            baseCameraEuler = playerCam.transform.localEulerAngles;
        }
        enemyLayer = LayerMask.GetMask("Enemy");

        if (bulletTracerPrefab != null && bulletTracerContainer != null)
        {
            tracerPool = new ObjectPool<BulletTracer>(bulletTracerPrefab, poolSize, bulletTracerContainer);
        }
        if (bulletHolePrefab != null && bulletHoleContainer != null)
        {
            impactPool = new ObjectPool<Effect>(bulletHolePrefab, poolSize, bulletHoleContainer);
        }
    }

    private void Update()
    {
        if (Time.time - lastFireTime > stabilizationDelay)
        {
            currentRecoil = Mathf.Lerp(currentRecoil, 0f, Time.deltaTime * stabilizationSpeed);
            currentSpread = Mathf.Lerp(currentSpread, baseSpread, Time.deltaTime * stabilizationSpeed);
        }

        currentRecoil = Mathf.Lerp(currentRecoil, targetRecoil, Time.deltaTime * stabilizationSpeed);
        targetRecoil = Mathf.Lerp(targetRecoil, 0f, Time.deltaTime * stabilizationSpeed);

        Vector3 weaponEuler = gameObject.transform.localEulerAngles;
        weaponEuler.z = currentRecoil;
        gameObject.transform.localEulerAngles = weaponEuler;
    }

    public void Fire()
    {
        CurrentAmmo--;
        OnShot?.Invoke();

        shotCounter++;

        float recoilThisShot = baseRecoil + (shotCounter - 1) * recoilIncreasePerShot;
        targetRecoil += recoilThisShot;
        if (targetRecoil > maxRecoil) targetRecoil = maxRecoil;

        currentSpread = Mathf.Min(currentSpread + spreadIncreasePerShot, maxSpread);

        Vector3 fireDirection = CalculateFireDirection();
        CreateBulletTracer(fireDirection);

        if (playerCam != null && Physics.Raycast(playerCam.transform.position, fireDirection, out RaycastHit hit, 100f, enemyLayer))
        {
            CreateImpactEffect(hit);

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

    private void CreateBulletTracer(Vector3 direction)
    {
        if (tracerPool != null && bulletTracerPrefab != null && muzzlePoint != null)
        {
            var tracer = tracerPool.Get();
            tracer.transform.position = muzzlePoint.position;
            tracer.transform.rotation = Quaternion.LookRotation(direction);
            tracer.GetComponent<BulletTracer>().Initialize(muzzlePoint.position, direction);

            StartCoroutine(ReturnToPoolAfterDelay(tracer, tracerDuration, tracerPool));
        }
    }

    private void CreateImpactEffect(RaycastHit hit)
    {
        if (impactPool != null && bulletHolePrefab != null)
        {
            var impact = impactPool.Get();
            impact.transform.position = hit.point;
            impact.transform.rotation = Quaternion.LookRotation(hit.normal);
            impact.transform.SetParent(hit.collider.transform);

            StartCoroutine(ReturnToPoolAfterDelay(impact, impactEffectDuration, impactPool));
        }
    }

    private IEnumerator ReturnToPoolAfterDelay(Effect obj, float delay, ObjectPool<Effect> pool)
    {
        yield return new WaitForSeconds(delay);
        pool.Return(obj);
    }

    private IEnumerator ReturnToPoolAfterDelay(BulletTracer obj, float delay, ObjectPool<BulletTracer> pool)
    {
        yield return new WaitForSeconds(delay);
        pool.Return(obj);
    }

    private Vector3 CalculateFireDirection()
    {
        if (playerCam == null) return Vector3.forward;

        Vector3 baseDirection = playerCam.transform.forward;
        /*
        float randomHorizontal = Random.Range(-currentSpread, currentSpread);
        float randomVertical = Random.Range(-currentSpread * 0.5f, currentSpread * 0.5f);
        Quaternion spreadRotation = Quaternion.Euler(randomVertical, randomHorizontal, 0f);
        Vector3 spreadDirection = spreadRotation * baseDirection;

        return spreadDirection.normalized;
        */
        return baseDirection;
    }

    public void ResetRecoil()
    {
        currentRecoil = 0f;
        targetRecoil = 0f;
        currentSpread = baseSpread;
        shotCounter = 0;
    }

    public void Reload()
    {
        isReloading = true;
        ResetRecoil();

        if (anim != null)
        {
            float speedMultiplier = reloadClip.length / ReloadTime;
            anim.speed = speedMultiplier;
            anim.Play("reload");
        }

        Invoke(nameof(FinishReload), ReloadTime);
    }

    private void FinishReload()
    {
        if (anim != null)
        {
            anim.speed = 1f;
        }
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

    private void OnDestroy()
    {
        tracerPool?.Clear();
        impactPool?.Clear();
    }
}