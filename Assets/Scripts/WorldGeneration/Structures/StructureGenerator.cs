using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.IO.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class StructureGenerator : NetworkBehaviour, ChunkGenerator, IPersistentData
{
    [SerializeField] int prewarmGOAmount = 15;
    [SerializeField] int chunkSize;
    [SerializeField] int midPoint;
    [SerializeField] GameObject structurePrefab;
    [SerializeField] StructureSO[] frontStructures;
    [SerializeField] StructureSO[] backStructures;
    private int leftX;
    private int rightX;
    private Dictionary<Vector2Int, List<StructureData>> loadedStructureData = new();
    private readonly Stack<Structure> inactiveStructures = new();

    public override void OnNetworkSpawn()
    {
        for (int i = 0; i < prewarmGOAmount; i++)
        {
            SpawnInactiveStructure();
        }
    }

    void SpawnInactiveStructure()
    {
        GameObject inactiveStructure = Instantiate(structurePrefab, transform);
        inactiveStructure.SetActive(false);
        Structure emptyStructure = inactiveStructure.GetComponent<Structure>();
        inactiveStructures.Push(emptyStructure);
    }

    Structure GetInactiveStructure()
    {
        if (inactiveStructures.Count == 0)
            SpawnInactiveStructure();
        return inactiveStructures.Pop();
    }

    void ReturnStructure(Structure structure)
    {
        structure.gameObject.SetActive(false);
        inactiveStructures.Push(structure);
    }

    [Rpc(SendTo.Server)]
    void UpdateStructureDataServerRpc(int structureIndex, bool isFront, Vector2 spawnLocation, Quaternion spawnRotation, int chunkX, int chunkY)
    {
        UpdateStructureDataClientRpc(structureIndex, isFront, spawnLocation, spawnRotation, chunkX, chunkY);
    }
    [Rpc(SendTo.ClientsAndHost)]
    void UpdateStructureDataClientRpc(int structureIndex, bool isFront, Vector2 spawnLocation, Quaternion spawnRotation, int chunkX, int chunkY)
    {
        StructureSO structureSO;
        if (isFront)
            structureSO = frontStructures[structureIndex];
        else
            structureSO = backStructures[structureIndex];
        Vector2Int chunkLocation = new(chunkX, chunkY);
        StructureData structureData = new()
        {
            StructureSO = structureSO,
            IsFront = isFront,
            StructureSOIndex = structureIndex,
            SpawnLocation = spawnLocation,
            SpawnRotation = spawnRotation
        };

        if (!loadedStructureData.ContainsKey(chunkLocation))
            loadedStructureData[chunkLocation] = new List<StructureData>();

        loadedStructureData[chunkLocation].Add(structureData);
    }

    public void GenerateStructuresRight(System.Random random, int chunkX, int chunkY)
    {
        if (chunkX < rightX)
            return;
        for (int i = 0; i < chunkSize; i++)
        {
            float x = chunkX * chunkSize + i;
            float y = chunkY * chunkSize + random.Next(chunkSize / 2);
            int randomStructureIndex;
            StructureSO structureSO;
            bool isFront = chunkY > midPoint;
            if (isFront)
            {
                randomStructureIndex = GetRandomFrontStructureIndex(random);
                structureSO = frontStructures[randomStructureIndex];
            }
            else
            {
                randomStructureIndex = GetRandomBackStructureIndex(random);
                structureSO = backStructures[randomStructureIndex];
                y++;
            }

            Structure spawnedStructure = SpawnStructureRight(structureSO, x, y);
            if (spawnedStructure == null)
                continue;

            Vector2 spawnLocation = spawnedStructure.transform.position;
            Quaternion spawnRotation = spawnedStructure.transform.rotation;

            UpdateStructureDataServerRpc(randomStructureIndex, isFront, spawnLocation, spawnRotation, chunkX, chunkY);

            BoxCollider2D boxCollider2D = spawnedStructure.BoxCollider2D;
            int structureXLength = (int)(boxCollider2D.size.x * structurePrefab.transform.localScale.x);
            i += structureXLength - 1;
        }
        rightX = chunkX;
    }
    public void GenerateStructuresLeft(System.Random random, int chunkX, int chunkY)
    {
        if (chunkX > leftX)
            return;
        for (int i = 0; i < chunkSize; i++)
        {
            float x = chunkX * chunkSize - i;
            float y = chunkY * chunkSize + random.Next(chunkSize / 2);
            int randomStructureIndex;
            StructureSO structureSO;
            bool isFront = chunkY > midPoint;
            if (isFront)
            {
                randomStructureIndex = GetRandomFrontStructureIndex(random);
                structureSO = frontStructures[randomStructureIndex];
            }
            else
            {
                randomStructureIndex = GetRandomBackStructureIndex(random);
                structureSO = backStructures[randomStructureIndex];
                y++;
            }

            Structure spawnedStructure = SpawnStructureLeft(structureSO, x, y);
            if (spawnedStructure == null)
                continue;

            Vector2 spawnLocation = spawnedStructure.transform.position;
            Quaternion spawnRotation = spawnedStructure.transform.rotation;

            UpdateStructureDataServerRpc(randomStructureIndex, isFront, spawnLocation, spawnRotation, chunkX, chunkY);

            BoxCollider2D boxCollider2D = spawnedStructure.BoxCollider2D;
            int structureXLength = (int)(boxCollider2D.size.x * structurePrefab.transform.localScale.x);
            i += structureXLength - 1;
        }
        leftX = chunkX;
    }

    private Structure SpawnStructureRight(StructureSO structureSO, float x, float y)
    {
        float xOffset = 2;
        float yOffset = 2;
        Vector2 spawnLocation = new(x + xOffset, y + yOffset);
        Vector2 scale = structurePrefab.transform.localScale;
        Vector2 size = structureSO.ColliderSize * scale;
        Vector2 sizeOffset = structureSO.ColliderOffset * scale;
        float sizeMultiplier = 0.98f;
        Collider2D hitCollider = Physics2D.OverlapBox(spawnLocation + sizeOffset, size * sizeMultiplier, 0);

        if (hitCollider != null)
        {
            return null;
        }

        return SpawnStructure(structureSO, spawnLocation, Quaternion.identity);
    }
    private Structure SpawnStructureLeft(StructureSO structureSO, float x, float y)
    {
        float xOffset = 2;
        float yOffset = 2;
        Vector2 spawnLocation = new(x + xOffset, y + yOffset);
        Vector2 scale = structurePrefab.transform.localScale;
        Vector2 size = structureSO.ColliderSize * scale;
        Vector2 sizeOffset = structureSO.ColliderOffset * scale;
        float sizeMultiplier = 0.98f;
        Collider2D hitCollider = Physics2D.OverlapBox(new Vector2(spawnLocation.x - sizeOffset.x, spawnLocation.y + sizeOffset.y), size * sizeMultiplier, 0);

        if (hitCollider != null)
        {
            return null;
        }

        return SpawnStructure(structureSO, spawnLocation, Quaternion.Euler(0, 180, 0));
    }

    Structure SpawnStructure(StructureSO structureSO, Vector2 spawnLocation, Quaternion spawnRotation)
    {
        Structure structure = GetInactiveStructure();
        structure.SetStructureData(structureSO, spawnLocation, spawnRotation);
        structure.gameObject.SetActive(true);
        return structure;
    }

    private int GetRandomFrontStructureIndex(System.Random random)
    {
        return random.Next(frontStructures.Length);
        // int totalWeight = 0;
        // foreach (StructureEntry entry in frontStructures)
        // {
        //     totalWeight += entry.weight;
        // }

        // foreach (StructureEntry entry in frontStructures)
        // {
        //     int randomInt = random.Next(totalWeight);
        //     if (randomInt < entry.weight)
        //         return entry.prefab;
        //     totalWeight -= entry.weight;
        // }
        // return null;
    }
    private int GetRandomBackStructureIndex(System.Random random)
    {
        return random.Next(backStructures.Length);
        // int totalWeight = 0;
        // foreach (StructureEntry entry in backStructures)
        // {
        //     totalWeight += entry.weight;
        // }

        // foreach (StructureEntry entry in backStructures)
        // {
        //     int randomInt = random.Next(totalWeight);
        //     if (randomInt < entry.weight)
        //         return entry.prefab;
        //     totalWeight -= entry.weight;
        // }
        // return null;
    }

    public void LoadChunkAt(System.Random random, int chunkX, int chunkY, GenerateDirection generateDirection, RoadType roadType)
    {
        if (roadType != RoadType.NONE)
            return;
        Vector2Int chunkPos = new(chunkX, chunkY);
        if (loadedStructureData.TryGetValue(chunkPos, out List<StructureData> loadedDatas))
        {
            foreach (StructureData data in loadedDatas)
            {
                SpawnStructure(data.StructureSO, data.SpawnLocation, data.SpawnRotation);
            }
            return;
        }

        if (generateDirection == GenerateDirection.EAST)
        {
            GenerateStructuresRight(random, chunkX, chunkY);
        }
        else if (generateDirection == GenerateDirection.WEST)
        {
            GenerateStructuresLeft(random, chunkX, chunkY);
        }
    }

    public void UnloadChunkAt(int chunkX, int chunkY)
    {
        Vector2Int chunkPos = new(chunkX, chunkY);
        if (loadedStructureData.TryGetValue(chunkPos, out List<StructureData> loadedDatas))
        {
            foreach (StructureData data in loadedDatas)
            {
                if (data.SpawnedStructure == null)
                    continue;
                ReturnStructure(data.SpawnedStructure);
                data.SpawnedStructure = null;
            }
        }
    }

    public void LoadData(WorldData worldData)
    {
        loadedStructureData.Clear();
        foreach (KeyValuePair<int[], List<StructureData>> data in worldData.LoadedStructuresData)
        {
            int[] pos = data.Key;
            Vector2Int chunkPos = new(pos[0], pos[1]);
            List<StructureData> structureDataList = data.Value;

            foreach (StructureData structureData in structureDataList)
            {
                float[] spawnLocationJson = structureData.SpawnLocationJson;
                float[] spawnRotationJson = structureData.SpawnRotationJson;

                Vector3 spawnLocation = new(spawnLocationJson[0], spawnLocationJson[1], spawnLocationJson[2]);
                Quaternion spawnRotation = new(spawnRotationJson[0], spawnRotationJson[1], spawnRotationJson[2], spawnRotationJson[3]);

                structureData.SpawnLocation = spawnLocation;
                structureData.SpawnRotation = spawnRotation;

                structureData.StructureSO = structureData.IsFront ? frontStructures[structureData.StructureSOIndex] : backStructures[structureData.StructureSOIndex];
                UpdateStructureDataServerRpc(
                    structureData.StructureSOIndex,
                    structureData.IsFront,
                    structureData.SpawnLocation,
                    structureData.SpawnRotation,
                    chunkPos.x, chunkPos.y
                );
            }
        }
    }

    public void SaveData(ref WorldData worldData)
    {
        List<KeyValuePair<int[], List<StructureData>>> structureMap = new();
        foreach (Vector2Int chunkPos in loadedStructureData.Keys)
        {
            int[] pos = { chunkPos.x, chunkPos.y };
            List<StructureData> structureDataList = loadedStructureData[chunkPos];

            foreach (StructureData structureData in structureDataList)
            {
                Vector3 spawnLocation = structureData.SpawnLocation;
                Quaternion spawnRotation = structureData.SpawnRotation;

                float[] spawnLocationJson = { spawnLocation.x, spawnLocation.y, spawnLocation.z };
                float[] spawnRotationJson = { spawnRotation.x, spawnRotation.y, spawnRotation.z, spawnRotation.w };

                structureData.SpawnLocationJson = spawnLocationJson;
                structureData.SpawnRotationJson = spawnRotationJson;
            }
            structureMap.Add(new KeyValuePair<int[], List<StructureData>>(pos, structureDataList));
        }
        worldData.LoadedStructuresData = structureMap;
    }
}

[Serializable]
public class StructureEntry
{
    public GameObject prefab;
    [HideInInspector] public int weight;
}

[Serializable]
public class StructureData
{
    public int StructureSOIndex { get; set; }
    public bool IsFront { get; set; }
    // We need these because json.net can't serialize Vector or Quaternion
    // Or I'm just dumb as always
    public float[] SpawnLocationJson { get; set; }
    public float[] SpawnRotationJson { get; set; }
    [JsonIgnore] public Quaternion SpawnRotation { get; set; }
    [JsonIgnore] public Vector2 SpawnLocation { get; set; }
    [JsonIgnore] public StructureSO StructureSO { get; set; }
    [JsonIgnore] public Structure SpawnedStructure { get; set; }
}