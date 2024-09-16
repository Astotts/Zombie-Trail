using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    public Tilemap sidewalkTilemap;
    public TileBase[] sidewalks;
    public StructureGenerator structureGenerator;
    readonly Dictionary<Vector2Int, TileData> tiles = new();
    private Vector2Int currentLoadedRoadPos;
    private int seed;
    private float gridSize;
    void Start()
    {
        gridSize = tilemap.transform.localScale.x;
        SpawnTile(initialTile, 0, 0);
        currentLoadedRoadPos = new Vector2Int(0, 0);
        tiles[currentLoadedRoadPos] = initialTile;
    }

    void Update()
    {
        int cameraScaledXPos = (int)(cam.transform.position.x + maxWidth * gridSize);
        int currentScaledXPos = (int)(currentLoadedRoadPos.x * gridSize);

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

        TileData newTileData = tileData;
        for (int i = 1; i < (maxHeight + 1) / 2; i++)
        {
            TileEntry[] tileEntries = newTileData.possibleNorthTiles;
            if (tileEntries == null || tileEntries.Length == 0)
            {
                float scaledX = (x + offset.x) * gridSize - gridSize / 2;
                float scaledY = (y + offset.y + i) * gridSize - gridSize / 2;
                GenerateSidewalks(Mathf.CeilToInt(scaledX), Mathf.CeilToInt(scaledY));
                structureGenerator.GenerateStructures(seed, new Vector2(scaledX, scaledY));
                continue;
            }
            int newYPos = y + i;
            TileEntry randomTileEntry = GetRandomTileEntries(tileEntries, random);
            newTileData = randomTileEntry.tileData;
            SpawnTile(newTileData, x, newYPos);
        }

        for (int i = 1; i < (maxHeight + 1) / 2; i++)
        {
            TileEntry[] tileEntries = newTileData.possibleSouthTiles;
            if (tileEntries == null || tileEntries.Length == 0)
                break;
            int newYPos = y - i;
            TileEntry randomTileEntry = GetRandomTileEntries(tileEntries, random);
            newTileData = randomTileEntry.tileData;
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

        Vector3Int position = new(x + (int)offset.x, y + (int)offset.y, (int)offset.z);

        tilemap.SetTile(position, tileData);
    }

    public void GenerateSidewalks(int x, int y)
    {
        for (int offsetX = 0; offsetX < gridSize; offsetX++)
        {
            for (int offsetY = 0; offsetY < gridSize; offsetY++)
            {
                TileBase sideWalkTile = GetRandomSidewalk();
                Vector3Int tilePos = new(x + offsetX, y + offsetY, 0);
                sidewalkTilemap.SetTile(tilePos, sideWalkTile);
            }
        }
    }

    public TileBase GetRandomSidewalk()
    {
        System.Random random = new System.Random();
        return sidewalks[random.Next(sidewalks.Length)];
    }
}
