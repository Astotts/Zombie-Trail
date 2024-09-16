using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class StructureGenerator : MonoBehaviour
{
    public int chunkSize;
    public StructureEntry[] structures;
    public void GenerateStructures(int seed, Vector2 chunkLocation)
    {
        System.Random random = new System.Random((int)(seed + chunkLocation.x));
        float[,] noiseMap = Noise.GenerateNoiseMap(chunkSize, chunkSize, 1, seed, 10, 1, 10, chunkLocation);
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                float height = noiseMap[x, y];
                if (height < 0.5f)
                {
                    GameObject randomStructure = GetRandomStructure(random, y);
                    if (SpawnStructure(randomStructure, (int)(x + chunkLocation.x), (int)(y + chunkLocation.y)))
                    {
                        return;
                    }
                }
            }
        }
    }

    private bool SpawnStructure(GameObject prefab, int x, int y)
    {
        float yOffset = 1.5f;
        float xOffset = 1f;
        Vector2 spawnLocation = new(x + xOffset, y + yOffset);
        BoxCollider2D boxCollider2D = prefab.GetComponent<BoxCollider2D>();
        Vector2 size = boxCollider2D.size;
        Collider2D hitCollider = Physics2D.OverlapBox(new Vector2(spawnLocation.x, spawnLocation.y + size.y), size * boxCollider2D.gameObject.transform.localScale, 0);

        if (hitCollider != null)
        {
            return false;
        }

        GameObject structure = Instantiate(prefab, this.transform);
        structure.transform.position = spawnLocation;
        return true;
    }

    private GameObject GetRandomStructure(System.Random random, int y)
    {
        int totalWeight = 0;
        foreach (StructureEntry entry in structures)
        {
            totalWeight += entry.weight;
        }

        foreach (StructureEntry entry in structures)
        {
            int randomInt = random.Next(totalWeight);
            if (randomInt < entry.weight)
                return entry.prefab;
            totalWeight -= entry.weight;
        }
        return null;
    }
}

[Serializable]
public class StructureEntry
{
    public GameObject prefab;
    public int yLevel;
    public int weight;
}