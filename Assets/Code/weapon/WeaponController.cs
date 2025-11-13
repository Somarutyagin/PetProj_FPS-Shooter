using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WeaponController : MonoBehaviour
{
    [Inject] private WeaponConfigsContainer _configsContainer;
    [Inject] private WeaponFactory _weaponFactory;
    [Inject] private AmmoViewModelFactory _viewModelFactory;
    
    private List<Weapon> _weapons = new List<Weapon>();
    private List<AmmoViewModel> _ammoViewModels = new List<AmmoViewModel>();
    private List<Vector3> _normalWeaponPos = new List<Vector3>();
    private int _activeWeaponIndex = 0;
    
    public event System.Action<AmmoViewModel> OnActiveWeaponChanged;
    public AmmoViewModel ActiveAmmoViewModel => _ammoViewModels[_activeWeaponIndex];

    private IInputProvider inputProvider;

    private Camera playerCamera;

    private const float aimingFov = 40f;
    private const float normalFov = 60f;
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
            var weapon = _weaponFactory.Create(config, playerCamera.gameObject.transform);
            _weapons.Add(weapon);
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

    public void SwitchToWeapon(int index)
    {
        if (index < 0 || index >= _weapons.Count)
        {
            Debug.LogWarning($"Invalid weapon index: {index}");
            return;
        }

        for (int i = 0; i < _weapons.Count; i++)
        {
            if (i == index)
            {
                _weapons[i].gameObject.SetActive(true);
            }
            else
            {
                _weapons[i].gameObject.SetActive(false);
            }
        }

        _activeWeaponIndex = index;
        OnActiveWeaponChanged?.Invoke(ActiveAmmoViewModel);
    }

    private void Update()
    {
        HandleFire();
        HandleReload();
        HandleADS();
        HandleSwitchWeapon();
    }

    private void HandleFire()
    {
        if (inputProvider.IsFirePressed() && _weapons[_activeWeaponIndex].CanFire())
        {
            if (_weapons.Count > 0) _weapons[_activeWeaponIndex].Fire();
        }
    }

    private void HandleReload()
    {
        if (inputProvider.IsReloadPressed() && _weapons[_activeWeaponIndex].CanReload())
        {
            _weapons[_activeWeaponIndex].Reload();
        }
    }

    private void HandleADS()
    {
        bool isAiming = Input.GetMouseButton(1);

        float targetFOV = isAiming ? aimingFov : normalFov;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * 5f);

        Vector3 targetWeaponPos = isAiming ? new Vector3(0, _normalWeaponPos[_activeWeaponIndex].y, _normalWeaponPos[_activeWeaponIndex].z) : _normalWeaponPos[_activeWeaponIndex];
        transform.GetChild(0).GetChild(_activeWeaponIndex).localPosition = Vector3.Lerp(transform.GetChild(0).GetChild(_activeWeaponIndex).localPosition, targetWeaponPos, Time.deltaTime * 5f);
    }

    private void HandleSwitchWeapon()
    {
        if (inputProvider.FirstGunPressed() && _activeWeaponIndex != 0)
        {
            SwitchToWeapon(0);
        }
        else if (inputProvider.SecondGunPressed() && _activeWeaponIndex != 1)
        {
            SwitchToWeapon(1);
        }    
    }
}
