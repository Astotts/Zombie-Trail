using System;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(menuName = "Car/Stats", fileName = "New Car Stats")]
public class CarStats : ScriptableObject
{
    // These will show up in inspector for entering base stats
    [SerializeField] private string _carName;
    [SerializeField] private string _carDescription;
    [SerializeField] private Sprite _carSprite;
    [SerializeField] private List<UpgradableStat> health;
    [SerializeField] private List<UpgradableStat> damage;
    [SerializeField] private List<UpgradableStat> capacity;
    [SerializeField] private List<UpgradableStat> speed;

    public string CarName => _carName;
    public string CarDescription => _carDescription;
    public Sprite CarSprite => _carSprite;

    // Other scripts can read these but can't modify
    public int GetStat(ECarStatName statName, int upgradeLevel)
    {
        return statName switch
        {
            ECarStatName.HEALTH => health[upgradeLevel].Value,
            ECarStatName.DAMAGE => damage[upgradeLevel].Value,
            ECarStatName.SPEED => speed[upgradeLevel].Value,
            ECarStatName.CAPACITY => capacity[upgradeLevel].Value,
            _ => -1,
        };
    }

    public int GetMaxLevel(ECarStatName statName)
    {
        return statName switch
        {
            ECarStatName.HEALTH => health.Count,
            ECarStatName.DAMAGE => damage.Count,
            ECarStatName.SPEED => speed.Count,
            ECarStatName.CAPACITY => capacity.Count,
            _ => -1,
        };
    }
}

[Serializable]
public class UpgradableStat
{
    [SerializeField] int value;
    [SerializeField] int cost;

    public int Value => value;
    public int Cost => cost;
}

public enum ECarStatName
{
    HEALTH,
    DAMAGE,
    SPEED,
    CAPACITY
}