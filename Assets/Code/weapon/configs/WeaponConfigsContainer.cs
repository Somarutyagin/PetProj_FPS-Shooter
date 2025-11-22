using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfigsContainer", menuName = "Scriptable Objects/WeaponConfigsContainer")]
public class WeaponConfigsContainer : ScriptableObject
{
    public List<WeaponConfig> WeaponConfigs = new List<WeaponConfig>();
}
