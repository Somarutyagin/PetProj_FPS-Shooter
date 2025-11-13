using UnityEngine;
using Zenject;

public class WeaponFactory : IFactory<WeaponConfig, Transform, Weapon>
{
    public Weapon Create(WeaponConfig config, Transform parent)
    {
        if (config == null) throw new System.ArgumentNullException(nameof(config));

        var weaponObj = Object.Instantiate(config.Prefab, parent);
        var weapon = weaponObj.AddComponent<Weapon>();

        weapon.Initialize(config.WeaponName, config.MaxAmmo, config.ReloadTime, config.FireRate, config.Damage);
        
        return weapon;
    }
}