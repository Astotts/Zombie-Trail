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
        System.Random random = new System.Random((int)(seed + chunkLocation.x + chunkLocation.y));
        for (int i = 0; i < chunkSize; i++)
        {
            float x = chunkLocation.x + i;
            float y = chunkLocation.y + random.Next(chunkSize / 2);
            GameObject randomStructure = GetRandomStructure(random);
            if (SpawnStructure(randomStructure, x, y))
            {
                BoxCollider2D boxCollider2D = randomStructure.GetComponent<BoxCollider2D>();
                float structureXLength = boxCollider2D.size.x * randomStructure.transform.localScale.x;
                i += (int)structureXLength - 1;
            }
        }
    }

    private bool SpawnStructure(GameObject prefab, float x, float y)
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
            return false;
        }

        GameObject structure = Instantiate(prefab, this.transform);
        structure.transform.position = spawnLocation;
        return true;
    }

    private GameObject GetRandomStructure(System.Random random)
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
    public int weight;
}