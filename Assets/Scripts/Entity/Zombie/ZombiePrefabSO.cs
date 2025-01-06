using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Zombie/AllPrefabSO")]
public class ZombiePrefabSO : ScriptableObject
{
    [SerializeField] List<ZombiePrefab> zombiePrefabList = new();
    public Dictionary<EZombieType, GameObject> zombiePrefabMap;
    public Dictionary<EZombieType, ScriptableObject[]> zombieStatMap;

    void OnEnable()
    {
        zombiePrefabMap = new();
        zombieStatMap = new();
        foreach (ZombiePrefab zombiePrefab in zombiePrefabList)
        {
            zombiePrefabMap.Add(zombiePrefab.Type, zombiePrefab.Prefab);
            zombieStatMap.Add(zombiePrefab.Type, zombiePrefab.Stats);
        }
    }

    public GameObject GetZombiePrefabFromType(EZombieType zombieType)
    {
        return zombiePrefabMap[zombieType];
    }

    public int GetZombieStatIndex(EZombieType zombieType, ScriptableObject zombieSO)
    {
        ScriptableObject[] zombieStats = zombieStatMap[zombieType];
        for (int i = 0; i < zombieStats.Length; i++)
        {
            if (zombieSO == zombieStats[i])
                return i;
        }
        return -1;
    }

    public ScriptableObject GetZombieStat(EZombieType zombieType, int index)
    {
        ScriptableObject[] zombieStats = zombieStatMap[zombieType];
        return zombieStats[index];
    }

    public ScriptableObject GetRandomZombieStats(EZombieType zombieType)
    {
        ScriptableObject[] zombieStats = zombieStatMap[zombieType];
        System.Random rand = new();
        return zombieStats[rand.Next(zombieStats.Length)];
    }
}

[Serializable]
public class ZombiePrefab
{
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public EZombieType Type { get; private set; }
    [field: SerializeField] public ScriptableObject[] Stats { get; private set; }
}