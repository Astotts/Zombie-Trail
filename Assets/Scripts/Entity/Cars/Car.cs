using System.Collections.Generic;
using UnityEngine;

public class Car
{
    CarStats stats;
    Dictionary<ECarStatName, int> upgrades;

    public Car(CarStats stats)
    {
        this.stats = stats;
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

    public Sprite GetSprite()
    {
        return stats.CarSprite;
    }
}
