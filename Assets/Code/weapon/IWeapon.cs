using UnityEngine;

public interface IWeapon
{
    void Fire();
    void Reload();
    bool CanFire();
    int GetAmmo();
    int GetMaxAmmoInfo();
    void SetAmmo(int ammo);
}
