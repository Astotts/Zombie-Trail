using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class LightPoleGenerator : MonoBehaviour, IChunkGenerator
{
    [SerializeField] int chunkSize;
    [SerializeField] List<GameObject> pooledObjects;
    [SerializeField] GameObject[] northLightPoles;
    [SerializeField] GameObject[] southLightPoles;
    [SerializeField] GameObject[] eastLightPoles;
    [SerializeField] GameObject[] westLightPoles;
    readonly Dictionary<Vector2Int, List<LightData>> loadedChunks = new();
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
        List<LightData> lightPolesAtChunk = new();
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

                LightData spawnedTopLightPole = SpawnLightPole(random, GenerateDirection.NORTH, x, topY);
                LightData spawnedBottomLightPole = SpawnLightPole(random, GenerateDirection.SOUTH, x, bottomY);

                lightPolesAtChunk.Add(spawnedTopLightPole);
                lightPolesAtChunk.Add(spawnedBottomLightPole);
                break;
            case RoadType.ROAD_VERTICAL:
                float leftX = chunkX * chunkSize;
                float rightX = chunkX * chunkSize + chunkSize;
                float y = chunkY * chunkSize + chunkSize / 2;

                LightData spawnedLeftLightPole = SpawnLightPole(random, GenerateDirection.EAST, leftX, y);
                LightData spawnedRightLightPole = SpawnLightPole(random, GenerateDirection.WEST, rightX, y);
                lightPolesAtChunk.Add(spawnedLeftLightPole);
                lightPolesAtChunk.Add(spawnedRightLightPole);
                break;
        }

        if (lightPolesAtChunk.Count == 0)
            return;

        Vector2Int chunkPos = new(chunkX, chunkY);
        loadedChunks.Add(chunkPos, lightPolesAtChunk);
    }

    LightData SpawnLightPole(System.Random random, GenerateDirection generateDirection, float x, float y)
    {
        Vector2 spawnLocation = new(x, y);
        GameObject lightPolePrefab = GetRandomLightPole(random, generateDirection);
        LightData data = SpawnLight(lightPolePrefab, spawnLocation, Quaternion.identity);

        return data;
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
        if (!loadedChunks.TryGetValue(chunkPos, out List<LightData> loadedLightPoles))
            return;

        foreach (LightData data in loadedLightPoles)
        {
            DespawnLight(data);
        }
        loadedChunks.Remove(chunkPos);
    }
}