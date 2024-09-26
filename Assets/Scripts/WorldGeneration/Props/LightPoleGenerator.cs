using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LightPoleGenerator : MonoBehaviour, ChunkGenerator
{
    [SerializeField] int chunkSize;
    [SerializeField] GameObject[] northLightPoles;
    [SerializeField] GameObject[] southLightPoles;
    [SerializeField] GameObject[] eastLightPoles;
    [SerializeField] GameObject[] westLightPoles;
    readonly Dictionary<Vector2Int, List<GameObject>> loadedChunks = new();
    public void LoadChunkAt(System.Random random, int chunkX, int chunkY, GenerateDirection generateDirection, RoadType roadType)
    {
        List<GameObject> lightPolesAtChunk = new();
        switch (roadType)
        {
            case RoadType.NONE:
            case RoadType.CROSS_INTERSECTION:
            case RoadType.CROSSWALK_EAST:
            case RoadType.CROSSWALK_NORTH:
            case RoadType.CROSSWALK_SOUTH:
            case RoadType.CROSSWALK_WEST:
                return;
            case RoadType.ROAD_HORIZONTAL:
                float topY = chunkY * chunkSize + chunkSize;
                float bottomY = chunkY * chunkSize;
                float x = chunkX * chunkSize + chunkSize / 2;

                GameObject spawnedTopLightPole = SpawnLightPole(random, GenerateDirection.NORTH, x, topY);
                GameObject spawnedBottomLightPole = SpawnLightPole(random, GenerateDirection.SOUTH, x, bottomY);

                lightPolesAtChunk.Add(spawnedTopLightPole);
                lightPolesAtChunk.Add(spawnedBottomLightPole);
                break;
            case RoadType.ROAD_VERTICAL:
                float leftX = chunkX * chunkSize;
                float rightX = chunkX * chunkSize + chunkSize;
                float y = chunkY * chunkSize + chunkSize / 2;

                GameObject spawnedLeftLightPole = SpawnLightPole(random, GenerateDirection.EAST, leftX, y);
                GameObject spawnedRightLightPole = SpawnLightPole(random, GenerateDirection.WEST, rightX, y);
                lightPolesAtChunk.Add(spawnedLeftLightPole);
                lightPolesAtChunk.Add(spawnedRightLightPole);
                break;
        }

        if (lightPolesAtChunk.Count == 0)
            return;

        Vector2Int chunkPos = new(chunkX, chunkY);
        loadedChunks.Add(chunkPos, lightPolesAtChunk);
    }

    GameObject SpawnLightPole(System.Random random, GenerateDirection generateDirection, float x, float y)
    {
        Vector3 spawnLocation = new(x, y);
        GameObject lightPolePrefab = GetRandomLightPole(random, generateDirection);
        GameObject spawnedLightPole = Instantiate(lightPolePrefab, transform);
        spawnedLightPole.transform.position = spawnLocation;

        return spawnedLightPole;
    }

    GameObject GetRandomLightPole(System.Random random, GenerateDirection direction)
    {
        return direction switch
        {
            GenerateDirection.NORTH => northLightPoles[random.Next(northLightPoles.Length)],
            GenerateDirection.SOUTH => southLightPoles[random.Next(southLightPoles.Length)],
            GenerateDirection.EAST => eastLightPoles[random.Next(eastLightPoles.Length)],
            GenerateDirection.WEST => westLightPoles[random.Next(westLightPoles.Length)],
            _ => null,
        };
    }

    public void UnloadChunkAt(int chunkX, int chunkY)
    {
        Vector2Int chunkPos = new(chunkX, chunkY);
        if (!loadedChunks.TryGetValue(chunkPos, out List<GameObject> loadedLightPoles))
            return;

        foreach (GameObject lightPoleGO in loadedLightPoles)
        {
            Destroy(lightPoleGO);
        }
        loadedChunks.Remove(chunkPos);
    }
}