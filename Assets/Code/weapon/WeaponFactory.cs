using UnityEngine;
using Zenject;

public class WeaponFactory : IFactory<WeaponConfig, GameObject, GameObject, Transform, PlayerHealthHandler, StatsManager, Weapon>
{
    public Weapon Create(WeaponConfig config, GameObject bulletTracerPrefab, GameObject bulletHolePrefab, Transform parent, PlayerHealthHandler playerHealth, StatsManager statsManager)
    {
        if (config == null) throw new System.ArgumentNullException(nameof(config));

        var weaponObj = Object.Instantiate(config.Prefab, parent);
        var weapon = weaponObj.AddComponent<Weapon>();

        weapon.Initialize(config.WeaponName, config.MaxAmmo, config.ReloadTime, config.FireRate, config.Damage, bulletTracerPrefab, bulletHolePrefab, playerHealth, statsManager);
        
        return weapon;
    }
}