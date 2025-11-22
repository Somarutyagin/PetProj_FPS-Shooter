using System.Collections.Generic;
using UnityEngine;

public class RarityManager
{
    private static readonly Dictionary<Rarity, float> BaseDropChances = new Dictionary<Rarity, float>
    {
        { Rarity.Common, 50.0f },
        { Rarity.Uncommon, 25.0f },
        { Rarity.Rare, 15.0f },
        { Rarity.Epic, 7.0f },
        { Rarity.Mythic, 2.5f },
        { Rarity.Legendary, 0.5f }
    };

    private static readonly Dictionary<Rarity, Color> RarityColors = new Dictionary<Rarity, Color>
    {
        { Rarity.Common, Color.gray },
        { Rarity.Uncommon, Color.green },
        { Rarity.Rare, Color.blue },
        { Rarity.Epic, Color.magenta},
        { Rarity.Mythic, Color.red },
        { Rarity.Legendary, Color.yellow }
    };

    public float GetRarityChance(Rarity rarity)
    {
        return BaseDropChances[rarity];
    }

    public Color GetRarityColor(Rarity rarity)
    {
        return RarityColors[rarity];
    }

    public Rarity GenerateRandomRarity()
    {
        float totalChance = 0f;
        foreach (var chance in BaseDropChances.Values)
        {
            totalChance += chance;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalChance);
        float cumulative = 0f;

        foreach (var kvp in BaseDropChances)
        {
            cumulative += kvp.Value;
            if (randomValue <= cumulative)
            {
                return kvp.Key;
            }
        }

        return Rarity.Common;
    }
}
