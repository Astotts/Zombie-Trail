using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    public int maxWidth;
    public int maxHeight;
    public Vector3 offset;
    public TileData initialTile;
    public GameObject cam;
    // I avoided GetComponent()
    public SpriteRenderer prefabSpriteRenderer;
    public GameObject tilePrefab;
    public Tilemap tilemap;
    Dictionary<Vector2Int, TileData> tiles = new();
    private Vector2Int currentLoadedRoadPos;
    private float gridSize;
    private int seed;
    void Start()
    {
        gridSize = prefabSpriteRenderer.bounds.size.x;
        SpawnTile(initialTile, 0, 0);
        currentLoadedRoadPos = new Vector2Int(0, 0);
        tiles[currentLoadedRoadPos] = initialTile;
    }

    void Update()
    {
        int cameraScaledXPos = (int)(cam.transform.position.x + maxWidth);
        int currentScaledXPos = (int)(currentLoadedRoadPos.x);
        if (currentScaledXPos <= cameraScaledXPos)
        {
            GenerateHorizontalRoad();
        }
    }

    void GenerateHorizontalRoad()
    {
        int x = currentLoadedRoadPos.x;
        int y = currentLoadedRoadPos.y;
        System.Random random = new System.Random(seed + x);
        Vector2Int newPos = new Vector2Int(x + 1, y);

        TileEntry[] possibleTiles = tiles[currentLoadedRoadPos].possibleEastTiles;
        TileEntry randomTileEntry = GetRandomTileEntries(possibleTiles, random);
        TileData newTileData = randomTileEntry.tileData;

        tiles[newPos] = newTileData;
        SpawnTile(newTileData, x, y);

        GenerateVerticalTiles(newTileData, x, y);

        currentLoadedRoadPos = newPos;
    }

    private void GenerateVerticalTiles(TileData tileData, int x, int y)
    {
        System.Random random = new System.Random(seed + y);

        for (int i = 0; i < maxHeight / 2; i++)
        {
            int newYPos = y + i;
            TileEntry[] tileEntries = tileData.possibleNorthTiles;
            if (tileEntries == null || tileEntries.Length == 0)
                return;
            TileEntry randomTileEntry = GetRandomTileEntries(tileEntries, random);
            TileData newTileData = randomTileEntry.tileData;
            SpawnTile(newTileData, x, newYPos);
        }

        for (int i = 0; i < maxHeight / 2; i++)
        {
            int newYPos = y - i;
            TileEntry[] tileEntries = tileData.possibleSouthTiles;
            if (tileEntries == null || tileEntries.Length == 0)
                return;
            TileEntry randomTileEntry = GetRandomTileEntries(tileEntries, random);
            TileData newTileData = randomTileEntry.tileData;
            SpawnTile(newTileData, x, newYPos);
        }
    }
    private TileEntry GetRandomTileEntries(TileEntry[] tileEntries, System.Random random)
    {
        int totalWeight = 0;
        foreach (TileEntry entry in tileEntries)
        {
            totalWeight += entry.weight;
        }

        foreach (TileEntry entry in tileEntries)
        {
            int randomInt = random.Next(totalWeight);
            if (randomInt < entry.weight)
                return entry;
            totalWeight -= entry.weight;
        }
        return tileEntries[0];
    }

    public void SpawnTile(TileData tileData, int x, int y)
    {
        // Vector3 worldLocation = new Vector3(x, y, 0);
        // worldLocation += offset;
        // worldLocation *= gridSize;
        // prefabSpriteRenderer.sprite = sprite;
        // GameObject gameObject = Instantiate(tilePrefab, this.transform);
        // gameObject.transform.position = worldLocation;

        Vector3Int position = new Vector3Int(x + (int)offset.x, y + (int)offset.y, (int)offset.z);

        tilemap.SetTile(position, tileData);
    }
}
