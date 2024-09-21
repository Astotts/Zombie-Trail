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
        if (roadType == RoadType.NONE)
            return;

        switch (generateDirection)
        {
            case GenerateDirection.NORTH:
            case GenerateDirection.SOUTH:
                {
                    float leftX = chunkX * chunkSize - 1.5f;
                    float rightX = chunkX * chunkSize + 1.5f;
                    float y = chunkY * chunkSize + 0.5f;

                    SpawnLightPole(random, generateDirection, leftX, y);
                    SpawnLightPole(random, generateDirection, rightX, y);
                    break;
                }
            case GenerateDirection.EAST:
            case GenerateDirection.WEST:
                {
                    float topY = chunkY * chunkSize + 1.5f;
                    float bottomY = chunkY * chunkSize - 1.5f;
                    float x = chunkX * chunkSize;

                    GameObject spawnedTopLightPole = SpawnLightPole(random, generateDirection, x, topY);
                    GameObject spawnedBottomLightPole = SpawnLightPole(random, generateDirection, x, bottomY);

                    AddLightPoleToDictionary(spawnedTopLightPole, chunkX, chunkY);
                    AddLightPoleToDictionary(spawnedBottomLightPole, chunkX, chunkY);
                    break;
                }
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
    }
}
