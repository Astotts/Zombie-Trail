using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DecorationGenerator : MonoBehaviour, ChunkGenerator
{
    [SerializeField] int chunkSize;
    [SerializeField] int zLevel;
    [SerializeField] Tilemap tilemap;
    [SerializeField] TileBase[] decorationTiles;
    readonly Dictionary<Vector3Int, TileBase> generatedDecorations = new();
    public void LoadChunkAt(System.Random random, int chunkX, int chunkY, GenerateDirection generateDirection, RoadType roadType)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                int xPos = chunkX * chunkSize + x;
                int yPos = chunkY * chunkSize + y;
                double chance = random.NextDouble();
                if (chance < 0.4f)
                    SetRandomDecorationAt(random, xPos, yPos);
            }
        }
    }
    void SetRandomDecorationAt(System.Random random, int x, int y)
    {
        Vector3Int pos = new(x, y, zLevel);

        if (generatedDecorations.TryGetValue(pos, out TileBase generatedDecoration))
        {
            tilemap.SetTile(pos, generatedDecoration);
            return;
        }

        TileBase randomDecoration = GetRandomDecorationTile(random);
        Matrix4x4 matrix4X4 = Matrix4x4.Translate(new Vector3((float)random.NextDouble() / 2, (float)random.NextDouble() / 2, 0));
        tilemap.SetTile(pos, randomDecoration);
        tilemap.SetTransformMatrix(pos, matrix4X4);
        generatedDecorations.Add(pos, randomDecoration);
    }

    TileBase GetRandomDecorationTile(System.Random random)
    {
        if (decorationTiles.Length == 0)
            return null;

        return decorationTiles[random.Next(decorationTiles.Length)];
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