using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] ZombieSpawnerStatsSO stats;

    public void SpawnBloater()
    {
        SpawnZombie(ZombieID.Bloater, transform.position);
    }
    public void SpawnSpitter()
    {
        SpawnZombie(ZombieID.Spitter, transform.position);
    }
    public void SpawnMelee()
    {
        SpawnZombie(ZombieID.Melee, transform.position);
    }

    public void SpawnZombie(ZombieID id, Vector2 spawnPos)
    {
        GameObject prefab = stats.GetZombie(id);
        SpawnZombieAt(prefab, spawnPos, Quaternion.identity);
    }

    void SpawnZombieAt(GameObject prefab, Vector2 spawnPos, Quaternion spawnRotation)
    {
        NetworkObject networkObject = NetworkObjectPool.Singleton.GetNetworkObject(prefab, spawnPos, spawnRotation);

        networkObject.Spawn();
    }
}

public enum ZombieID
{
    Bloater,
    Spitter,
    Melee
}