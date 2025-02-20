using System.Collections.Generic;
using UnityEngine;

public class Car
{
    public bool IsUnlocked = false;
    readonly CarStats stats;
    readonly Dictionary<ECarStatName, int> upgrades = new();

    public Car(CarStats stats)
    {
        this.stats = stats;

        upgrades[ECarStatName.HEALTH] = 0;
        upgrades[ECarStatName.DAMAGE] = 0;
        upgrades[ECarStatName.SPEED] = 0;
        upgrades[ECarStatName.CAPACITY] = 0;
    }

    public bool Upgrade(ECarStatName statName)
    {
        int currentLevel = upgrades[statName];
        if (currentLevel >= stats.GetMaxLevel(statName))
            return false;

        upgrades[statName] = currentLevel + 1;
        return true;
    }

    public int GetStat(ECarStatName statName)
    {
        return stats.GetStat(statName, upgrades[statName]);
    }

    public int GetUpgradeLevel(ECarStatName statName)
    {
        return upgrades[statName];
    }

    public Sprite GetSprite()
    {
        return stats.CarSprite;
    }

    public string GetName()
    {
        return stats.CarName;
    }
}
