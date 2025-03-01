using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    public int maxWidth;
    public int maxHeight;
    [SerializeField] int seed;
    [SerializeField] RoadTile initialTile;
    [SerializeField] Transform cam;
    [SerializeField] Tilemap tilemap5x5;
    [SerializeField] List<GameObject> generatorGameObjects = new();
    readonly Dictionary<Vector2Int, RoadTile> tiles = new();
    private Vector2Int currentRightRoadPos;
    private Vector2Int currentLeftRoadPos;
    private System.Random random;
    private readonly List<IChunkGenerator> chunkGenerators = new();
    [HideInInspector] public float chunkSize;
    void Awake()
    {
        foreach (GameObject generatorGO in generatorGameObjects)
        {
            IChunkGenerator generator = generatorGO.GetComponent<IChunkGenerator>();
            chunkGenerators.Add(generator);
        }
        chunkSize = tilemap5x5.transform.localScale.x;
    }
    void Start()
    {
        random = new(seed);
        int x = 0;
        int y = 0;
        SetRoadTile(initialTile, x, y);
        GenerateVerticalTiles(initialTile, x, y, GenerateDirection.EAST);
        currentRightRoadPos = new Vector2Int(x, y);
        currentLeftRoadPos = new Vector2Int(x, y);
        tiles[currentRightRoadPos] = initialTile;
        LoadChunkGenerators(x, y, GenerateDirection.EAST, initialTile.GetRoadType());
    }

    void FixedUpdate()
    {
        GenerateRightHorizontalRoad();
        GenerateLeftHorizontalRoad();
    }

    void GenerateRightHorizontalRoad()
    {
        int cameraRightBound = (int)(cam.position.x / chunkSize + maxWidth);
        int x = currentRightRoadPos.x;
        if (x > cameraRightBound)
        {
            UnloadChunkAt(currentRightRoadPos);
            currentRightRoadPos.x--;
        }
        else if (x < cameraRightBound)
        {
            currentRightRoadPos.x++;
            LoadChunkAt(currentRightRoadPos, GenerateDirection.EAST);
        }
    }
    void GenerateLeftHorizontalRoad()
    {
        int cameraLeftBound = (int)(cam.position.x / chunkSize - maxWidth);
        int x = currentLeftRoadPos.x;
        if (x < cameraLeftBound)
        {
            UnloadChunkAt(currentLeftRoadPos);
            currentLeftRoadPos.x++;
        }
        else if (x > cameraLeftBound)
        {
            currentLeftRoadPos.x--;
            LoadChunkAt(currentLeftRoadPos, GenerateDirection.WEST);
        }
    }

    void UnloadChunkAt(Vector2Int pos)
    {
        Vector3Int roadPos = new(pos.x, pos.y, 0);
        tilemap5x5.SetTile(roadPos, null);
        UnloadChunkGenerators(pos.x, pos.y);

        for (int i = 1; i < maxHeight; i++)
        {
            int northY = pos.y + i;
            int southY = pos.y - i;

            Vector3Int northPos = new(pos.x, northY, 0);
            Vector3Int southPos = new(pos.x, southY, 0);
            tilemap5x5.SetTile(northPos, null);
            tilemap5x5.SetTile(southPos, null);
            UnloadChunkGenerators(pos.x, northY);
            UnloadChunkGenerators(pos.x, southY);
        }
    }

    void UnloadChunkGenerators(int x, int y)
    {
        foreach (IChunkGenerator chunkGenerator in chunkGenerators)
        {
            chunkGenerator.UnloadChunkAt(x, y);
        }
    }

    void LoadChunkAt(Vector2Int pos, GenerateDirection generateDirection)
    {
        random = new(seed + pos.x * maxWidth * 2 + pos.y);
        if (!tiles.TryGetValue(pos, out RoadTile roadTile))
        {
            if (generateDirection == GenerateDirection.WEST)
                GenerateLeftHorizontalRoadAt(currentLeftRoadPos);
            else
                GenerateRightHorizontalRoadAt(currentRightRoadPos);
            return;
        }
        SetRoadTile(roadTile, pos.x, pos.y);
        LoadChunkGenerators(pos.x, pos.y, generateDirection, roadTile.GetRoadType());
        GenerateVerticalTiles(roadTile, pos.x, pos.y, generateDirection);

    }

    void GenerateRightHorizontalRoadAt(Vector2Int newPos)
    {
        Vector2Int prevPos = new(newPos.x - 1, newPos.y);

        RoadEntry[] possibleTiles = tiles[prevPos].possibleEastTiles;
        RoadEntry randomTileEntry = GetRandomRoadEntry(possibleTiles);
        RoadTile newTileData = randomTileEntry.tileData;

        tiles[newPos] = newTileData;
        SetRoadTile(newTileData, newPos.x, newPos.y);
        LoadChunkGenerators(newPos.x, newPos.y, GenerateDirection.EAST, newTileData.GetRoadType());

        GenerateVerticalTiles(newTileData, newPos.x, newPos.y, GenerateDirection.EAST);
    }

    void GenerateLeftHorizontalRoadAt(Vector2Int newPos)
    {
        Vector2Int prevPos = new(newPos.x + 1, newPos.y);

        RoadEntry[] possibleTiles = tiles[prevPos].possibleWestTiles;
        RoadEntry randomTileEntry = GetRandomRoadEntry(possibleTiles);
        RoadTile newTileData = randomTileEntry.tileData;

        tiles[newPos] = newTileData;
        SetRoadTile(newTileData, newPos.x, newPos.y);
        LoadChunkGenerators(newPos.x, newPos.y, GenerateDirection.WEST, newTileData.GetRoadType());

        GenerateVerticalTiles(newTileData, newPos.x, newPos.y, GenerateDirection.WEST);
    }

    void LoadChunkGenerators(int chunkX, int chunkY, GenerateDirection generateDirection, RoadType roadType)
    {
        foreach (IChunkGenerator chunkGenerator in chunkGenerators)
        {
            System.Random chunkRandom = new(seed + chunkX * maxWidth + chunkY);
            chunkGenerator.LoadChunkAt(chunkRandom, chunkX, chunkY, generateDirection, roadType);
        }
    }

    void GenerateVerticalTiles(RoadTile roadTile, int x, int y, GenerateDirection direction)
    {
        RoadTile northRoadTile = roadTile;
        RoadTile southRoadTile = roadTile;
        for (int i = 1; i < maxHeight; i++)
        {
            int northY = y + i;
            int southY = y - i;
            RoadEntry[] northEntries = northRoadTile.possibleNorthTiles;
            RoadEntry[] southEntries = southRoadTile.possibleSouthTiles;

            if (northEntries == null || northEntries.Length == 0)
            {
                LoadChunkGenerators(x, northY, direction, RoadType.NONE);
            }
            else
            {
                RoadEntry randomNorthEntry = GetRandomRoadEntry(northEntries);
                northRoadTile = randomNorthEntry.tileData;
                SetRoadTile(northRoadTile, x, northY);
                LoadChunkGenerators(x, northY, direction, northRoadTile.GetRoadType());
            }
            if (southEntries == null || southEntries.Length == 0)
            {
                LoadChunkGenerators(x, southY, direction, RoadType.NONE);
            }
            else
            {
                RoadEntry randomSouthEntry = GetRandomRoadEntry(southEntries);
                southRoadTile = randomSouthEntry.tileData;
                SetRoadTile(southRoadTile, x, southY);
                LoadChunkGenerators(x, southY, direction, southRoadTile.GetRoadType());
            }
        }
    }
    RoadEntry GetRandomRoadEntry(RoadEntry[] tileEntries)
    {
        int totalWeight = 0;
        foreach (RoadEntry entry in tileEntries)
        {
            totalWeight += entry.weight;
        }

        foreach (RoadEntry entry in tileEntries)
        {
            int randomInt = random.Next(totalWeight);
            if (randomInt < entry.weight)
                return entry;
            totalWeight -= entry.weight;
        }
        return tileEntries[0];
    }

    void SetRoadTile(RoadTile tileData, int x, int y)
    {
        Vector3Int position = new(x, y, 0);
        tilemap5x5.SetTile(position, tileData);
    }
}

[Serializable]
public class DecorationEntry
{
    public TileBase tile;
    public int weight;
}