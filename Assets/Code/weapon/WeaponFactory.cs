using UnityEngine;
using Zenject;

public class WeaponFactory : IFactory<WeaponConfig, GameObject, GameObject, Transform, Weapon>
{
    public Weapon Create(WeaponConfig config, GameObject bulletTracerPrefab, GameObject bulletHolePrefab, Transform parent)
    {
        if (config == null) throw new System.ArgumentNullException(nameof(config));

        var weaponObj = Object.Instantiate(config.Prefab, parent);
        var weapon = weaponObj.AddComponent<Weapon>();

        weapon.Initialize(config.WeaponName, config.MaxAmmo, config.ReloadTime, config.FireRate, config.Damage, bulletTracerPrefab, bulletHolePrefab);
        
        return weapon;
    }
}