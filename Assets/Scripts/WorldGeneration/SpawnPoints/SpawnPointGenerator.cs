using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPointGenerator : NetworkBehaviour, IChunkGenerator
{
    [SerializeField] int spawnCount;
    [SerializeField] int chunkSize;
    [SerializeField] float zLevel;
    public readonly Dictionary<Vector2Int, List<Vector3>> spawnPoints = new();
    public override void OnNetworkSpawn()
    {
        if (!IsHost)
            return;
        base.OnNetworkSpawn();
    }
    public void LoadChunkAt(System.Random random, int chunkX, int chunkY, GenerateDirection generateDirection, RoadType roadType)
    {
        if (roadType == RoadType.NONE)
            return;

        float minX = chunkX * chunkSize;
        float maxX = chunkX * chunkSize + chunkSize;

        float minY = chunkY * chunkSize;
        float maxY = chunkY * chunkSize + chunkSize;

        Vector2Int chunkLocation = new(chunkX, chunkY);
        List<Vector3> pointsInChunk = new();

        for (int i = 0; i < spawnCount; i++)
        {
            float randomX = UnityEngine.Random.Range(minX, maxX);
            float randomY = UnityEngine.Random.Range(minY, maxY);

            Vector3 randomLocation = new(randomX, randomY, zLevel);
            pointsInChunk.Add(randomLocation);
        }

        spawnPoints.Add(chunkLocation, pointsInChunk);
    }

    public void UnloadChunkAt(int chunkX, int chunkY)
    {
        Vector2Int chunkLocation = new(chunkX, chunkY);
        spawnPoints.Remove(chunkLocation);
    }

    public List<Vector3> GetAllSpawnPoints()
    {
        List<Vector3> finalList = new();

        foreach (List<Vector3> spawnPointList in spawnPoints.Values)
        {
            foreach (Vector3 spawnPoint in spawnPointList)
            {
                finalList.Add(spawnPoint);
            }
        }

        return finalList;
    }
}