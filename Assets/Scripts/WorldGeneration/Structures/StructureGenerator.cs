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

public class StructureGenerator : NetworkBehaviour, IChunkGenerator
{
    [SerializeField] int prewarmGOAmount = 15;
    [SerializeField] int chunkSize;
    [SerializeField] int midPoint;
    [SerializeField] GameObject structurePrefab;
    [SerializeField] StructureSO[] frontStructures;
    [SerializeField] StructureSO[] backStructures;
    private int leftX;
    private int rightX;
    private Quaternion generalRotation;
    private readonly Dictionary<Vector2Int, List<StructureData>> loadedStructureData = new();

    private readonly Stack<Structure> inactiveStructures = new();

    public override void OnNetworkSpawn()
    {
        generalRotation = Quaternion.identity;
        for (int i = 0; i < prewarmGOAmount; i++)
        {
            Structure structure = SpawnStructureGO();
            structure.gameObject.SetActive(false);
        }
    }

    Structure SpawnStructureGO()
    {
        GameObject inactiveStructure = Instantiate(structurePrefab, transform);
        Structure emptyStructure = inactiveStructure.GetComponent<Structure>();
        emptyStructure.SetStructureGenerator(this);
        return emptyStructure;
    }

    Structure SpawnStructureGO(Vector2 spawnPos, Quaternion spawnRot)
    {
        GameObject inactiveStructure = Instantiate(structurePrefab, spawnPos, spawnRot, transform);
        Structure emptyStructure = inactiveStructure.GetComponent<Structure>();
        emptyStructure.SetStructureGenerator(this);
        return emptyStructure;
    }

    Structure GetStructure(Vector2 spawnPos, Quaternion spawnRot)
    {
        if (inactiveStructures.TryPop(out Structure structure))
        {
            structure.transform.SetPositionAndRotation(spawnPos, spawnRot);
            structure.gameObject.SetActive(true);
            return structure;
        }

        return SpawnStructureGO(spawnPos, spawnRot);
    }

    public void ReturnStructure(Structure structure)
    {
        structure.gameObject.SetActive(false);
        inactiveStructures.Push(structure);
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
            Vector2Int chunkPos = new(chunkX, chunkY);

            StructureData data = new()
            {
                structureSO = structureSO,
                spawnPosition = spawnedStructure.transform.position,
                spawnRotation = spawnedStructure.transform.rotation,
                spawnedStructure = spawnedStructure
            };

            if (loadedStructureData.TryGetValue(chunkPos, out List<StructureData> dataList))
            {
                dataList.Add(data);
                spawnedStructure.SetStructureList(dataList, data);
            }
            else
            {
                List<StructureData> newDataList = new()
                {
                    data
                };
                spawnedStructure.SetStructureList(newDataList, data);
                loadedStructureData[chunkPos] = newDataList;
            }

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
            Vector2Int chunkPos = new(chunkX, chunkY);

            StructureData data = new()
            {
                structureSO = structureSO,
                spawnPosition = spawnedStructure.transform.position,
                spawnRotation = spawnedStructure.transform.rotation,
                spawnedStructure = spawnedStructure
            };

            if (loadedStructureData.TryGetValue(chunkPos, out List<StructureData> dataList))
            {
                dataList.Add(data);
                spawnedStructure.SetStructureList(dataList, data);
            }
            else
            {
                List<StructureData> newDataList = new()
                {
                    data
                };
                spawnedStructure.SetStructureList(newDataList, data);
                loadedStructureData[chunkPos] = newDataList;
            }

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

        return SpawnStructure(structureSO, spawnLocation, generalRotation);
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
        Structure structure = GetStructure(spawnLocation, spawnRotation);
        structure.SetStructureData(structureSO);
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
                Structure structure = GetStructure(data.spawnPosition, data.spawnRotation);
                structure.SetStructureData(data.structureSO);
                data.spawnedStructure = structure;
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
        Debug.Log("Are you unloading at all?");
        if (loadedStructureData.TryGetValue(chunkPos, out List<StructureData> loadedDatas))
        {
            Debug.Log("Soooo, They exist");
            foreach (StructureData data in loadedDatas)
            {
                ReturnStructure(data.spawnedStructure);
            }
        }
    }
}


public class StructureData
{
    public StructureSO structureSO;
    public Structure spawnedStructure;
    public Vector2 spawnPosition;
    public Quaternion spawnRotation;
}