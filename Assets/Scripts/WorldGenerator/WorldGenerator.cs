using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private Vector2Int startPos;
    [SerializeField] private Vector2 loadExtends;
    [SerializeField] private IntVariable chunkSize;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private List<ChunkGenerator> weightedChunkGenerators = new();
    
    private readonly Dictionary<string, object> chunkData = new();
    private Transform cameraTransform;
    private int currentRightX;
    private int currentLeftX;

    void Awake()
    {
        currentLeftX = currentRightX = startPos.x;
    }

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleRight();
    }

    void HandleRight()
    {
        int cameraRightBound = (int)(cameraTransform.position.x / chunkSize.Value + loadExtends.x);
        if (currentRightX < cameraRightBound)
        {
            Vector2 chunkPos = new(currentRightX, startPos.y);
            LoadChunk(chunkPos);
            LoadVerticalChunks(currentRightX);
            currentRightX++;
        }
        else if (currentRightX > cameraRightBound)
        {
            // TODO Unload
        }
    }

    void LoadVerticalChunks(int x)
    {
        for (int i = 0; i < loadExtends.y; i++)
        {
            Vector2 top = new(x, startPos.y + i);
            Vector2 bottom = new(x, startPos.y - i);

            LoadChunk(top);
            LoadChunk(bottom);
        }
    }

    void LoadChunk(Vector2 chunkPos)
    {
        foreach (ChunkGenerator chunkGenerator in weightedChunkGenerators)
        {
            chunkGenerator.OnChunkLoad(chunkPos, tilemap, chunkData);
        }
    }
}