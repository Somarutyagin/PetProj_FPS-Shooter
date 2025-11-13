using System;
using UnityEngine;
using Zenject;

public class AmmoViewModel : IDisposable
{
    public event Action<int, int> OnAmmoChanged;
    public AmmoModel model;
    private Weapon _weapon;
    
    [Inject]
    public AmmoViewModel(AmmoModel model, Weapon weapon)
    {
        this.model = model;
        _weapon = weapon;
        
        model.SetAmmo(_weapon.MaxAmmo, _weapon.MaxAmmo);
        
        _weapon.OnShot += OnWeaponShot;
        _weapon.OnReload += OnWeaponReload;
        
        NotifyView();
    }
    private void OnWeaponShot()
    {
        Shoot();
    }
    
    private void OnWeaponReload(int amount)
    {
        Reload(amount);
    }

    public void Shoot()
    {
        model.Shoot();
        NotifyView();
    }

    public void Reload(int amount)
    {
        model.Reload(amount);
        NotifyView();
    }

    private void NotifyView()
    {
        OnAmmoChanged?.Invoke(model.CurrentAmmo, model.MaxAmmo);
    }
    public void Dispose()
    {
        if (_weapon != null)
        {
            _weapon.OnShot -= OnWeaponShot;
            _weapon.OnReload -= OnWeaponReload;
        }
    }
}