using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightGenerator : MonoBehaviour, ChunkGenerator
{
    [SerializeField] float zLevel;
    [SerializeField] Vector3 northOffset;
    [SerializeField] Vector3 southOffset;
    [SerializeField] Vector3 eastOffset;
    [SerializeField] Vector3 westOffset;
    [SerializeField] WorldGenerator worldGenerator;
    [SerializeField] GameObject[] northTrafficLights;
    [SerializeField] GameObject[] southTrafficLights;
    [SerializeField] GameObject[] eastTrafficLights;
    [SerializeField] GameObject[] westTrafficLights;
    private float chunkSize;
    private readonly Dictionary<Vector2Int, List<GameObject>> generatedTrafficLights = new();
    void Start()
    {
        chunkSize = worldGenerator.chunkSize;
    }
    public void LoadChunkAt(System.Random random, int chunkX, int chunkY, GenerateDirection generateDirection, RoadType roadType)
    {
        Vector3 chunkLocation = new(chunkX * chunkSize, chunkY * chunkSize, zLevel);
        List<GameObject> trafficLightAtChunk = new();
        switch (roadType)
        {
            case RoadType.CROSS_INTERSECTION:
                {
                    Vector3 northPos = chunkLocation + northOffset;
                    GameObject spawnedNorth = SpawnTrafficLight(random, GenerateDirection.NORTH, northPos);
                    trafficLightAtChunk.Add(spawnedNorth);

                    Vector3 southPos = chunkLocation + southOffset;
                    GameObject spawnedSouth = SpawnTrafficLight(random, GenerateDirection.SOUTH, southPos);
                    trafficLightAtChunk.Add(spawnedSouth);

                    Vector3 eastPos = chunkLocation + eastOffset;
                    GameObject spawnedEast = SpawnTrafficLight(random, GenerateDirection.EAST, eastPos);
                    trafficLightAtChunk.Add(spawnedEast);

                    Vector3 westPos = chunkLocation + westOffset;
                    GameObject spawnedWest = SpawnTrafficLight(random, GenerateDirection.WEST, westPos);
                    trafficLightAtChunk.Add(spawnedWest);
                }
                break;
            case RoadType.T_INTERSECTION_DOWN:
                {
                    Vector3 southPos = chunkLocation + southOffset;
                    GameObject spawnedSouth = SpawnTrafficLight(random, GenerateDirection.SOUTH, southPos);
                    trafficLightAtChunk.Add(spawnedSouth);

                    Vector3 eastPos = chunkLocation + eastOffset;
                    GameObject spawnedEast = SpawnTrafficLight(random, GenerateDirection.EAST, eastPos);
                    trafficLightAtChunk.Add(spawnedEast);

                    Vector3 westPos = chunkLocation + westOffset;
                    GameObject spawnedWest = SpawnTrafficLight(random, GenerateDirection.WEST, westPos);
                    trafficLightAtChunk.Add(spawnedWest);
                }
                break;
            case RoadType.T_INTERSECTION_UP:
                {
                    Vector3 northPos = chunkLocation + northOffset;
                    GameObject spawnedNorth = SpawnTrafficLight(random, GenerateDirection.NORTH, northPos);
                    trafficLightAtChunk.Add(spawnedNorth);

                    Vector3 eastPos = chunkLocation + eastOffset;
                    GameObject spawnedEast = SpawnTrafficLight(random, GenerateDirection.EAST, eastPos);
                    trafficLightAtChunk.Add(spawnedEast);

                    Vector3 westPos = chunkLocation + westOffset;
                    GameObject spawnedWest = SpawnTrafficLight(random, GenerateDirection.WEST, westPos);
                    trafficLightAtChunk.Add(spawnedWest);
                }
                break;
            default:
                return;
        }
    }


    GameObject SpawnTrafficLight(System.Random random, GenerateDirection generateDirection, Vector3 spawnLocation)
    {
        GameObject TrafficLightPrefab = GetRandomTrafficLight(random, generateDirection);
        GameObject spawnedTrafficLight = Instantiate(TrafficLightPrefab, transform);
        spawnedTrafficLight.transform.position = spawnLocation;

        return spawnedTrafficLight;
    }

    GameObject SpawnTrafficLightAt(GameObject prefab, Vector3 spawnLocation)
    {
        GameObject spawnedGO = Instantiate(prefab, transform);
        spawnedGO.transform.position = spawnLocation;

        return spawnedGO;
    }

    GameObject GetRandomTrafficLight(System.Random random, GenerateDirection direction)
    {
        return direction switch
        {
            GenerateDirection.NORTH => northTrafficLights[random.Next(northTrafficLights.Length)],
            GenerateDirection.SOUTH => southTrafficLights[random.Next(southTrafficLights.Length)],
            GenerateDirection.EAST => eastTrafficLights[random.Next(eastTrafficLights.Length)],
            GenerateDirection.WEST => westTrafficLights[random.Next(westTrafficLights.Length)],
            _ => null,
        };
    }

    public void UnloadChunkAt(int chunkX, int chunkY)
    {

    }
}
