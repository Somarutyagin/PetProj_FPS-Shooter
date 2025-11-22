using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerStatsConfig", menuName = "Scriptable Objects/PlayerStatsConfig")]
public class PlayerStatsConfig : ScriptableObject
{
    [System.Serializable]
    public class StatConfig
    {
        public StatType Type;
        public float BaseValue;
        public float MaxValue = 100f;
        public float UpgradeBaseIncrement = 7f;
    }

    public List<StatConfig> Stats = new List<StatConfig>();
}
