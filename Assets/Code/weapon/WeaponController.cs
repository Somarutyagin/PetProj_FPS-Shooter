using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WeaponController : MonoBehaviour
{
    [Inject(Id = "BulletTracer")] private GameObject bulletTracerPrefab;
    [Inject(Id = "BulletHole")] private GameObject bulletHolePrefab;
    [Inject] private WeaponConfigsContainer _configsContainer;
    [Inject] private WeaponFactory _weaponFactory;
    [Inject] private AmmoViewModelFactory _viewModelFactory;
    
    private List<AmmoViewModel> _ammoViewModels = new List<AmmoViewModel>();
    private List<Vector3> _normalWeaponPos = new List<Vector3>();
    private int _activeWeaponIndex = 0;
    
    public event System.Action<AmmoViewModel> OnActiveWeaponChanged;
    public AmmoViewModel ActiveAmmoViewModel => _ammoViewModels[_activeWeaponIndex];

    private IInputProvider inputProvider;

    private Camera playerCamera;

    private const float aimingFov = 40f;
    private const float normalFov = 60f;

    private bool isSwitchingAnim = false;

    void Start()
    {
        playerCamera = Camera.main;
        inputProvider = GetComponent<IInputProvider>();

        var configs = _configsContainer.WeaponConfigs;
        if (configs == null || configs.Count == 0)
        {
            Debug.LogError("WeaponConfigs not set in WeaponConfigsContainer!");
            return;
        }

        int createdCounter = 0;

        foreach (var config in configs)
        {
            var weapon = _weaponFactory.Create(config, bulletTracerPrefab, bulletHolePrefab, playerCamera.gameObject.transform);
            _normalWeaponPos.Add(weapon.transform.localPosition);

            if (createdCounter == 0)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }

            var viewModel = _viewModelFactory.Create(weapon);
            _ammoViewModels.Add(viewModel);

            createdCounter++;
        }

        OnActiveWeaponChanged?.Invoke(ActiveAmmoViewModel);
    }
    private void OnDestroy()
    {
        foreach (var viewModel in _ammoViewModels)
        {
            viewModel?.Dispose();
        }
    }

    private async UniTask SwitchToWeaponAnim(int index)
    {
        isSwitchingAnim = true;

        ActiveAmmoViewModel.Weapon.PutAwayAnim();
        await UniTask.Delay((int)(0.2f * 1000), DelayType.DeltaTime);

        ActiveAmmoViewModel.Weapon.gameObject.SetActive(false);
        ActiveAmmoViewModel.Weapon.ResetRecoil();

        _activeWeaponIndex = index;
        OnActiveWeaponChanged?.Invoke(ActiveAmmoViewModel);

        ActiveAmmoViewModel.Weapon.gameObject.SetActive(true);

        ActiveAmmoViewModel.Weapon.GetItAnim();
        await UniTask.Delay((int)(0.2f * 1000), DelayType.DeltaTime);

        isSwitchingAnim = false;
    }

    private void Update()
    {
        if (!isSwitchingAnim)
        {
            HandleFire();
            HandleReload();
            HandleADS();
            HandleSwitchWeapon();
        }
    }
    private void HandleFire()
    {
        if (inputProvider.IsFirePressed() && ActiveAmmoViewModel.Weapon.CanFire())
        {
            ActiveAmmoViewModel.Weapon.Fire();
        }
    }

    private void HandleReload()
    {
        if (inputProvider.IsReloadPressed() && ActiveAmmoViewModel.Weapon.CanReload())
        {
            ActiveAmmoViewModel.Weapon.Reload();
        }
    }

    private void HandleADS()
    {
        bool isAiming = inputProvider.IsADSPressed();

        float targetFOV = isAiming ? aimingFov : normalFov;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * 5f);

        Vector3 targetWeaponPos = isAiming ? new Vector3(0, _normalWeaponPos[_activeWeaponIndex].y, _normalWeaponPos[_activeWeaponIndex].z) : _normalWeaponPos[_activeWeaponIndex];
        transform.GetChild(0).GetChild(_activeWeaponIndex).localPosition = Vector3.Lerp(transform.GetChild(0).GetChild(_activeWeaponIndex).localPosition, targetWeaponPos, Time.deltaTime * 5f);
    }

    private void HandleSwitchWeapon()
    {
        if (inputProvider.FirstGunPressed() && _activeWeaponIndex != 0)
        {
            SwitchToWeaponAnim(0).Forget();
        }
        else if (inputProvider.SecondGunPressed() && _activeWeaponIndex != 1)
        {
            SwitchToWeaponAnim(1).Forget();
        }    
    }
}
