using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Scriptable Objects/WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    public string WeaponName;
    public int MaxAmmo;
    public float ReloadTime;
    public float FireRate;
    public float Damage;
    public GameObject Prefab;
}
