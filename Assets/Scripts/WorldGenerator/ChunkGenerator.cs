using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class ChunkGenerator : ScriptableObject
{
    [field: SerializeField] public int Weight { get; private set; }
    [field: SerializeField] public IntVariable ChunkSize { get; private set; }
    public abstract void OnChunkLoad(Vector2Int chunkPos, Tilemap tilemap, Dictionary<string, object> currentData);
    public abstract void OnChunkUnload(Vector2Int chunkPos, Tilemap tilemap, Dictionary<string, object> currentData);
}