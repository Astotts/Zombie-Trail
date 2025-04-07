using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private Vector2Int startPos;
    [SerializeField] private Vector2Int loadExtends;
    [SerializeField] private IntVariable chunkSize;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private List<ChunkGenerator> weightedChunkGenerators = new();

    private int seed;
    private readonly Dictionary<string, object> chunkData = new();
    private Transform cameraTransform;
    private int currentRightX;
    private int currentLeftX;

    void Awake()
    {
        currentLeftX = startPos.x;
        currentRightX = currentLeftX + 1;

        weightedChunkGenerators.Sort((a, b) => {
            return a.Weight.CompareTo(b.Weight);
        });
    }

    void Start()
    {
        // seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        seed = 0;
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleLeft();
        HandleRight();
    }

    void HandleLeft()
    {
        int cameraLeftBound = (int)(cameraTransform.position.x / chunkSize.Value - loadExtends.x);
        if (currentLeftX > cameraLeftBound)
        {
            Vector2Int chunkPos = new(currentLeftX, startPos.y);
            LoadChunk(chunkPos);
            LoadVerticalChunks(currentLeftX);
            currentLeftX--;
        }
        else if (currentLeftX < cameraLeftBound)
        {
            Vector2Int chunkPos = new(currentLeftX, startPos.y);
            UnloadChunk(chunkPos);
            UnloadVerticalChunks(currentLeftX);
            currentLeftX++;
        }
    }

    void HandleRight()
    {
        int cameraRightBound = (int)(cameraTransform.position.x / chunkSize.Value + loadExtends.x);
        if (currentRightX < cameraRightBound)
        {
            Vector2Int chunkPos = new(currentRightX, startPos.y);
            LoadChunk(chunkPos);
            LoadVerticalChunks(currentRightX);
            currentRightX++;
        }
        else if (currentRightX > cameraRightBound)
        {
            Vector2Int chunkPos = new(currentRightX, startPos.y);
            UnloadChunk(chunkPos);
            UnloadVerticalChunks(currentRightX);
            currentRightX--;
        }
    }

    void LoadVerticalChunks(int x)
    {
        for (int i = 0; i < loadExtends.y; i++)
        {
            Vector2Int top = new(x, startPos.y + i);
            Vector2Int bottom = new(x, startPos.y - i);

            LoadChunk(top);
            LoadChunk(bottom);
        }
    }

    void UnloadVerticalChunks(int x)
    {
        for (int i = 0; i < loadExtends.y; i++)
        {
            Vector2Int top = new(x, startPos.y + i);
            Vector2Int bottom = new(x, startPos.y - i);

            UnloadChunk(top);
            UnloadChunk(bottom);
        }
    }

    void LoadChunk(Vector2Int chunkPos)
    {
        // Debug.Log("Loading chunk at " + chunkPos);
        foreach (ChunkGenerator chunkGenerator in weightedChunkGenerators)
        {
            chunkGenerator.OnChunkLoad(seed, chunkPos, tilemap, chunkData);
        }
    }

    void UnloadChunk(Vector2Int chunkPos)
    {
        // Debug.Log("Unloading chunk at " + chunkPos);
        foreach (ChunkGenerator chunkGenerator in weightedChunkGenerators)
        {
            chunkGenerator.OnChunkUnload(seed, chunkPos, tilemap, chunkData);
        }
    }
}