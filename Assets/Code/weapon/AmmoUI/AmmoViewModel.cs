using System;
using Zenject;
using UniRx;

public class AmmoViewModel : IDisposable
{
    public AmmoModel Model { get; }
    public Weapon Weapon { get; }
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    [Inject]
    public AmmoViewModel(AmmoModel model, Weapon weapon)
    {
        Model = model;
        Weapon = weapon;

        Model.SetAmmo(Weapon.MaxAmmo, Weapon.MaxAmmo);

        Weapon.OnShot += OnWeaponShot;
        Weapon.OnReload += OnWeaponReload;
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
        Model.Shoot();
    }

    public void Reload(int amount)
    {
        Model.Reload(amount);
    }

    public void Dispose()
    {
        if (Weapon != null)
        {
            Weapon.OnShot -= OnWeaponShot;
            Weapon.OnReload -= OnWeaponReload;
        }
        _disposables.Dispose();
    }
}