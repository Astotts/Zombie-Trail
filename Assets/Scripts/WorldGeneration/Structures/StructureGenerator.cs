using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class StructureGenerator : MonoBehaviour, ChunkGenerator
{
    public int chunkSize;
    public GameObject[] frontStructures;
    public GameObject[] backStructures;
    private int midPoint;
    private int leftX;
    private int rightX;
    public void GenerateStructuresRight(System.Random random, int chunkX, int chunkY)
    {
        if (chunkX < rightX)
            return;
        for (int i = 0; i < chunkSize; i++)
        {
            float x = chunkX * chunkSize + i;
            float y = chunkY * chunkSize + random.Next(chunkSize / 2);
            GameObject randomStructure;
            if (chunkY > midPoint)
            {
                randomStructure = GetRandomFrontStructure(random);
            }
            else
            {
                randomStructure = GetRandomBackStructure(random);
                y++;
            }
            GameObject spawnedStructure = SpawnStructureRight(randomStructure, x, y);
            if (spawnedStructure != null)
            {
                BoxCollider2D boxCollider2D = randomStructure.GetComponent<BoxCollider2D>();
                int structureXLength = (int)(boxCollider2D.size.x * randomStructure.transform.localScale.x);
                i += structureXLength - 1;
            }
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
            GameObject randomStructure;
            if (chunkY > midPoint)
            {
                randomStructure = GetRandomFrontStructure(random);
            }
            else
            {
                randomStructure = GetRandomBackStructure(random);
                y++;
            }
            GameObject spawnedStructure = SpawnStructureLeft(randomStructure, x, y);
            if (spawnedStructure != null)
            {
                BoxCollider2D boxCollider2D = randomStructure.GetComponent<BoxCollider2D>();
                int structureXLength = (int)(boxCollider2D.size.x * randomStructure.transform.localScale.x);
                i += structureXLength - 1;
            }
        }
        leftX = chunkX;
    }

    private GameObject SpawnStructureRight(GameObject prefab, float x, float y)
    {
        float xOffset = 2;
        float yOffset = 2;
        Vector2 spawnLocation = new(x + xOffset, y + yOffset);
        BoxCollider2D boxCollider2D = prefab.GetComponent<BoxCollider2D>();
        Vector3 scale = boxCollider2D.transform.localScale;
        Vector3 size = boxCollider2D.size * scale;
        Vector2 sizeOffset = boxCollider2D.offset * scale;
        float sizeMultiplier = 0.98f;
        Collider2D hitCollider = Physics2D.OverlapBox(spawnLocation + sizeOffset, size * sizeMultiplier, 0);

        if (hitCollider != null)
        {
            return null;
        }

        GameObject structure = Instantiate(prefab, this.transform);
        structure.transform.position = spawnLocation;
        return structure;
    }
    private GameObject SpawnStructureLeft(GameObject prefab, float x, float y)
    {
        float xOffset = 2;
        float yOffset = 2;
        Vector2 spawnLocation = new(x + xOffset, y + yOffset);
        BoxCollider2D boxCollider2D = prefab.GetComponent<BoxCollider2D>();
        Vector3 scale = boxCollider2D.transform.localScale;
        Vector3 size = boxCollider2D.size * scale;
        Vector2 sizeOffset = boxCollider2D.offset * scale;
        float sizeMultiplier = 0.98f;
        Collider2D hitCollider = Physics2D.OverlapBox(new Vector2(spawnLocation.x - sizeOffset.x, spawnLocation.y + sizeOffset.y), size * sizeMultiplier, 0);

        if (hitCollider != null)
        {
            return null;
        }

        GameObject structure = Instantiate(prefab, this.transform);
        structure.transform.SetPositionAndRotation(spawnLocation + new Vector2(0, 0), Quaternion.Euler(0, 180, 0));
        return structure;
    }

    private GameObject GetRandomFrontStructure(System.Random random)
    {
        return frontStructures[random.Next(frontStructures.Length)];
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
    private GameObject GetRandomBackStructure(System.Random random)
    {
        return backStructures[random.Next(backStructures.Length)];
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
    }
}

[Serializable]
public class StructureEntry
{
    public GameObject prefab;
    [HideInInspector] public int weight;
}