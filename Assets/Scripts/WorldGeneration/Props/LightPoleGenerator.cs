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
    readonly Dictionary<Vector2Int, List<GameObject>> generatedObjects = new();
    public void LoadChunkAt(System.Random random, int chunkX, int chunkY, GenerateDirection generateDirection, RoadType roadType)
    {
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

                AddLightPoleToDictionary(spawnedTopLightPole, chunkX, chunkY);
                AddLightPoleToDictionary(spawnedBottomLightPole, chunkX, chunkY);
                break;
            case RoadType.ROAD_VERTICAL:
                float leftX = chunkX * chunkSize;
                float rightX = chunkX * chunkSize + chunkSize;
                float y = chunkY * chunkSize + chunkSize / 2;

                SpawnLightPole(random, GenerateDirection.EAST, leftX, y);
                SpawnLightPole(random, GenerateDirection.WEST, rightX, y);
                break;
        }
    }

    void AddLightPoleToDictionary(GameObject lightPole, int chunkX, int chunkY)
    {
        Vector2Int chunkPos = new(chunkX, chunkY);
        if (generatedObjects.TryGetValue(chunkPos, out List<GameObject> list))
        {
            list.Add(lightPole);
        }
        else
        {
            List<GameObject> newList = new() { lightPole };
            generatedObjects.Add(chunkPos, newList);
        }
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
        if (!generatedObjects.TryGetValue(chunkPos, out List<GameObject> loadedLightPoles))
            return;

        foreach (GameObject lightPoleGO in loadedLightPoles)
        {
            Destroy(lightPoleGO);
        }
        generatedObjects.Remove(chunkPos);
    }
}
