using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SidewalkGenerator : MonoBehaviour, IChunkGenerator
{
    [SerializeField] int zLevel;
    [SerializeField] int chunkSize;
    [SerializeField] Tilemap tilemap;
    [SerializeField] TileBase[] sidewalkTiles;
    private readonly Dictionary<Vector3Int, TileBase> generatedSidewalks = new();
    public void LoadChunkAt(System.Random random, int chunkX, int chunkY, GenerateDirection generateDirection, RoadType roadType)
    {
        if (roadType != RoadType.NONE)
            return;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                int xPos = chunkX * chunkSize + x;
                int yPos = chunkY * chunkSize + y;
                SetRandomSidewalkAt(random, xPos, yPos);
            }
        }
    }

    void SetRandomSidewalkAt(System.Random random, int x, int y)
    {
        Vector3Int pos = new(x, y, zLevel);

        if (generatedSidewalks.TryGetValue(pos, out TileBase generatedSidewalk))
        {
            tilemap.SetTile(pos, generatedSidewalk);
            return;
        }

        TileBase randomSidewalk = GetRandomSidewalkTile(random);
        tilemap.SetTile(pos, randomSidewalk);
        generatedSidewalks.Add(pos, randomSidewalk);
    }

    TileBase GetRandomSidewalkTile(System.Random random)
    {
        if (sidewalkTiles.Length == 0)
            return null;

        return sidewalkTiles[random.Next(sidewalkTiles.Length)];
    }

    public void UnloadChunkAt(int chunkX, int chunkY)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                int xPos = chunkX * chunkSize + x;
                int yPos = chunkY * chunkSize + y;
                Vector3Int pos = new(xPos, yPos, zLevel);
                tilemap.SetTile(pos, null);
            }
        }
    }
}