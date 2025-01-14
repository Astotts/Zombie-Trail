using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Spawner/Zombie", fileName = "New Zombie Spawner Stats")]
public class ZombieSpawnerStatsSO : ScriptableObject
{
    [field: SerializeField] public List<ZombieInfo> zombieList { get; private set; }

    public Dictionary<ZombieID, ZombieInfo> zombieMap = new();

    void OnEnable()
    {
        foreach (ZombieInfo zombie in zombieList)
        {
            zombieMap.Add(zombie.ID, zombie);
        }
    }

    public GameObject GetZombie(ZombieID id)
    {
        return zombieMap[id].Prefab;
    }

    public GameObject GetRandomZombiePrefab()
    {
        System.Random random = new();
        int totalWeight = 0;

        foreach (ZombieInfo zombieType in zombieList)
        {
            totalWeight += zombieType.Weight;
        }

        float randomWeight = random.Next(totalWeight);

        foreach (ZombieInfo zombieType in zombieList)
        {
            if (totalWeight - zombieType.Weight > randomWeight)
                return zombieType.Prefab;
        }

        return zombieList[0].Prefab;
    }
}

[Serializable]
public class ZombieInfo
{
    [field: SerializeField] public ZombieID ID { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public int Weight { get; private set; }
}