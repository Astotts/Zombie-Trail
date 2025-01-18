using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Spawner/Zombie", fileName = "New Zombie Spawner Stats")]
public class ZombieSpawnerStatsSO : ScriptableObject
{
    [field: SerializeField] public int BaseMaxZombie { get; private set; }
    [field: SerializeField] public int MaxZombiePerLevel { get; private set; }
    [field: SerializeField] public float BaseSpawnTime { get; private set; }
    [field: SerializeField] public float SpawnRatePerLevel { get; private set; }
    [field: SerializeField] public float BaseLeashTime { get; private set; }
    [field: SerializeField] public float LeashRatePerLevel { get; private set; }
    [field: SerializeField] public float DistanceBetweenOffsets { get; private set; }
    [field: SerializeField] public LayerMask LayerWhitelist { get; private set; }
    [field: SerializeField] public Vector2 SpawnAreaCenter { get; private set; }
    [field: SerializeField] public Vector2 SpawnAreaExtends { get; private set; }
    [field: SerializeField] public Vector2 LeashAreaCenter { get; private set; }
    [field: SerializeField] public Vector2 LeashAreaExtends { get; private set; }
    [field: SerializeField] public SpawnPool[] SpawnPools { get; private set; }

    public GameObject GetRandomZombiePrefab(int level)
    {
        System.Random random = new();
        int totalWeight = 0;

        List<ZombieInfo> zombieList = SpawnPools[Mathf.Min(level, SpawnPools.Length)].Zombies;
        foreach (ZombieInfo zombieType in zombieList)
        {
            totalWeight += zombieType.Weight;
        }

        float randomWeight = random.Next(totalWeight);

        foreach (ZombieInfo zombieType in zombieList)
        {
            if (totalWeight - zombieType.Weight >= randomWeight)
                return zombieType.Prefab;
        }

        return null;
    }

    public float GetSecondsPerSpawn(int level)
    {
        return BaseSpawnTime / (1 + SpawnRatePerLevel * (level % 3));
    }

    public int GetSpawnAmount(int level)
    {
        return level % 3;
    }

    public float GetSecondsPerLeash(int level)
    {
        return BaseLeashTime % (1 + LeashRatePerLevel * level);
    }

    public int GetMaxZombie(int level)
    {
        return BaseMaxZombie + MaxZombiePerLevel * level;
    }
}

[Serializable]
public class SpawnPool
{
    [field: SerializeField] public List<ZombieInfo> Zombies { get; private set; }
}

[Serializable]
public class ZombieInfo
{
    [field: SerializeField] public ZombieID ID { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public int Weight { get; private set; }
}