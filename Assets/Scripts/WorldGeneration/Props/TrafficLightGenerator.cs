using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TrafficLightGenerator : MonoBehaviour, ChunkGenerator
{
    [SerializeField] float zLevel;
    [SerializeField] Vector3 northOffset;
    [SerializeField] Vector3 southOffset;
    [SerializeField] Vector3 eastOffset;
    [SerializeField] Vector3 westOffset;
    [SerializeField] WorldGenerator worldGenerator;
    [SerializeField] List<GameObject> pooledObjects;
    [SerializeField] GameObject[] northTrafficLights;
    [SerializeField] GameObject[] southTrafficLights;
    [SerializeField] GameObject[] eastTrafficLights;
    [SerializeField] GameObject[] westTrafficLights;
    private float chunkSize;
    private readonly Dictionary<Vector2Int, List<LightData>> loadedChunks = new();
    readonly Dictionary<GameObject, Stack<LightData>> light2DPools = new();

    struct LightData
    {
        public GameObject Prefab { get; set; }
        public GameObject Spawned { get; set; }
        public Light2D[] Lights { get; set; }
    }

    void Awake()
    {
        foreach (GameObject go in pooledObjects)
        {
            light2DPools[go] = new();
        }
    }

    void Start()
    {
        chunkSize = worldGenerator.chunkSize;
        WaveManager.Instance.OnStateChange += OnWaveStateChange;
    }

    private void OnWaveStateChange(object sender, WaveState state)
    {
        if (state == WaveState.StartDay)
        {
            foreach (List<LightData> dataList in loadedChunks.Values)
            {
                foreach (LightData data in dataList)
                {
                    foreach (Light2D light in data.Lights)
                    {
                        light.enabled = false;
                    }
                }
            }
        }
        else if (state == WaveState.StartNight)
        {
            foreach (List<LightData> dataList in loadedChunks.Values)
            {
                foreach (LightData data in dataList)
                {
                    foreach (Light2D light in data.Lights)
                    {
                        light.enabled = true;
                    }
                }
            }
        }
    }

    LightData SpawnLight(GameObject prefab, Vector2 spawnPos, Quaternion spawnRot)
    {
        if (light2DPools[prefab].TryPop(out LightData lightData))
        {
            lightData.Spawned.SetActive(true);
            return lightData;
        }
        else
        {
            GameObject spawnedGO = Instantiate(prefab, spawnPos, spawnRot, transform);
            Light2D[] lights = spawnedGO.GetComponentsInChildren<Light2D>();

            LightData data = new()
            {
                Prefab = prefab,
                Spawned = spawnedGO,
                Lights = lights
            };

            return data;
        }
    }

    void DespawnLight(LightData data)
    {
        data.Spawned.SetActive(false);
        light2DPools[data.Prefab].Push(data);
    }

    public void LoadChunkAt(System.Random random, int chunkX, int chunkY, GenerateDirection generateDirection, RoadType roadType)
    {
        Vector3 chunkLocation = new(chunkX * chunkSize, chunkY * chunkSize, zLevel);
        List<LightData> trafficLightAtChunk = new();
        switch (roadType)
        {
            case RoadType.CROSS_INTERSECTION:
                {
                    Vector3 northPos = chunkLocation + northOffset;
                    LightData spawnedNorth = SpawnTrafficLight(random, GenerateDirection.NORTH, northPos);
                    trafficLightAtChunk.Add(spawnedNorth);

                    Vector3 southPos = chunkLocation + southOffset;
                    LightData spawnedSouth = SpawnTrafficLight(random, GenerateDirection.SOUTH, southPos);
                    trafficLightAtChunk.Add(spawnedSouth);

                    Vector3 eastPos = chunkLocation + eastOffset;
                    LightData spawnedEast = SpawnTrafficLight(random, GenerateDirection.EAST, eastPos);
                    trafficLightAtChunk.Add(spawnedEast);

                    Vector3 westPos = chunkLocation + westOffset;
                    LightData spawnedWest = SpawnTrafficLight(random, GenerateDirection.WEST, westPos);
                    trafficLightAtChunk.Add(spawnedWest);
                }
                break;
            case RoadType.T_INTERSECTION_DOWN:
                {
                    Vector3 southPos = chunkLocation + southOffset;
                    LightData spawnedSouth = SpawnTrafficLight(random, GenerateDirection.SOUTH, southPos);
                    trafficLightAtChunk.Add(spawnedSouth);

                    Vector3 eastPos = chunkLocation + eastOffset;
                    LightData spawnedEast = SpawnTrafficLight(random, GenerateDirection.EAST, eastPos);
                    trafficLightAtChunk.Add(spawnedEast);

                    Vector3 westPos = chunkLocation + westOffset;
                    LightData spawnedWest = SpawnTrafficLight(random, GenerateDirection.WEST, westPos);
                    trafficLightAtChunk.Add(spawnedWest);
                }
                break;
            case RoadType.T_INTERSECTION_UP:
                {
                    Vector3 northPos = chunkLocation + northOffset;
                    LightData spawnedNorth = SpawnTrafficLight(random, GenerateDirection.NORTH, northPos);
                    trafficLightAtChunk.Add(spawnedNorth);

                    Vector3 eastPos = chunkLocation + eastOffset;
                    LightData spawnedEast = SpawnTrafficLight(random, GenerateDirection.EAST, eastPos);
                    trafficLightAtChunk.Add(spawnedEast);

                    Vector3 westPos = chunkLocation + westOffset;
                    LightData spawnedWest = SpawnTrafficLight(random, GenerateDirection.WEST, westPos);
                    trafficLightAtChunk.Add(spawnedWest);
                }
                break;
            default:
                return;
        }
        Vector2Int chunkPos = new(chunkX, chunkY);
        loadedChunks.Add(chunkPos, trafficLightAtChunk);
    }


    LightData SpawnTrafficLight(System.Random random, GenerateDirection generateDirection, Vector3 spawnLocation)
    {
        GameObject prefab = GetRandomTrafficLight(random, generateDirection);
        LightData spawnedTrafficLight = SpawnLight(prefab, spawnLocation, Quaternion.identity);

        return spawnedTrafficLight;
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
        Vector2Int chunkPos = new(chunkX, chunkY);
        if (!loadedChunks.TryGetValue(chunkPos, out List<LightData> loadedTrafficLights))
            return;

        foreach (LightData data in loadedTrafficLights)
        {
            DespawnLight(data);
        }
        loadedChunks.Remove(chunkPos);
    }
}
