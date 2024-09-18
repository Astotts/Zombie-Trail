using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] int maxWidth;
    [SerializeField] int maxHeight;
    [SerializeField] int sidewalkZLevel;
    [SerializeField] int decorationZLevel;
    [SerializeField] Vector3 offset;
    [SerializeField] RoadTile initialTile;
    [SerializeField] GameObject cam;
    // I avoided GetComponent()
    [SerializeField] SpriteRenderer prefabSpriteRenderer;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Tilemap tilemap5x5;
    [SerializeField] Tilemap tilemap1x1;
    [SerializeField] TileBase[] sidewalks;
    [SerializeField] DecorationEntry[] roadDecorations;
    [SerializeField] DecorationEntry[] sidewalkDecorations;
    [SerializeField] StructureGenerator structureGenerator;
    readonly Dictionary<Vector2Int, RoadTile> tiles = new();
    private Vector2Int currentRightRoadPos;
    private Vector2Int currentLeftRoadPos;
    private int seed;
    private float gridSize;
    void Start()
    {
        System.Random random = new();
        seed = random.Next();
        gridSize = tilemap5x5.transform.localScale.x;
        int x = (int)offset.x;
        int y = (int)offset.y;
        SetRoadTile(initialTile, x, y);
        GenerateVerticalTiles(initialTile, x, y, GenerateDirection.RIGHT);
        currentRightRoadPos = new Vector2Int(x, y);
        currentLeftRoadPos = new Vector2Int(x, y);
        tiles[currentRightRoadPos] = initialTile;
    }

    void FixedUpdate()
    {
        GenerateRightHorizontalRoad();
        GenerateLeftHorizontalRoad();
    }

    void GenerateRightHorizontalRoad()
    {
        int cameraRightBound = (int)(cam.transform.position.x / gridSize + maxWidth);
        int x = currentRightRoadPos.x;
        if (x > cameraRightBound)
        {
            UnloadChunkAt(currentRightRoadPos);
            currentRightRoadPos.x--;
        }
        else if (x < cameraRightBound)
        {
            currentRightRoadPos.x++;
            LoadChunkAt(currentRightRoadPos, GenerateDirection.RIGHT);
        }
    }
    void GenerateLeftHorizontalRoad()
    {
        int cameraLeftBound = (int)(cam.transform.position.x / gridSize - maxWidth);
        int x = currentLeftRoadPos.x;
        if (x < cameraLeftBound)
        {
            UnloadChunkAt(currentLeftRoadPos);
            currentLeftRoadPos.x++;
        }
        else if (x > cameraLeftBound)
        {
            currentLeftRoadPos.x--;
            LoadChunkAt(currentLeftRoadPos, GenerateDirection.LEFT);
        }
    }

    private void LoadChunkAt(Vector2Int pos, GenerateDirection generateDirection)
    {
        if (!tiles.TryGetValue(pos, out RoadTile roadTile))
        {
            if (generateDirection == GenerateDirection.LEFT)
                GenerateLeftHorizontalRoadAt(currentLeftRoadPos);
            else
                GenerateRightHorizontalRoadAt(currentRightRoadPos);
            return;
        }
        SetRoadTile(roadTile, pos.x, pos.y);
        GenerateVerticalTiles(roadTile, pos.x, pos.y, generateDirection);
    }

    void GenerateRightHorizontalRoadAt(Vector2Int newPos)
    {
        System.Random random = new(seed - newPos.x);
        Vector2Int prevPos = new(newPos.x - 1, newPos.y);

        RoadEntry[] possibleTiles = tiles[prevPos].possibleEastTiles;
        RoadEntry randomTileEntry = GetRandomRoadEntry(possibleTiles, random);
        RoadTile newTileData = randomTileEntry.tileData;

        tiles[newPos] = newTileData;
        SetRoadTile(newTileData, newPos.x, newPos.y);

        GenerateVerticalTiles(newTileData, newPos.x, newPos.y, GenerateDirection.RIGHT);
    }

    void GenerateLeftHorizontalRoadAt(Vector2Int newPos)
    {
        System.Random random = new(seed - newPos.x);
        Vector2Int prevPos = new(newPos.x + 1, newPos.y);

        RoadEntry[] possibleTiles = tiles[prevPos].possibleWestTiles;
        RoadEntry randomTileEntry = GetRandomRoadEntry(possibleTiles, random);
        RoadTile newTileData = randomTileEntry.tileData;

        tiles[newPos] = newTileData;
        SetRoadTile(newTileData, newPos.x, newPos.y);

        GenerateVerticalTiles(newTileData, newPos.x, newPos.y, GenerateDirection.LEFT);
    }

    private void UnloadChunkAt(Vector2Int pos)
    {
        Vector3Int roadPos = new(pos.x, pos.y, 0);
        float scaledX = pos.x * gridSize - gridSize / 2;
        float scaledY = pos.y * gridSize - gridSize / 2;
        tilemap5x5.SetTile(roadPos, null);
        UnloadDecorationAt((int)scaledX, (int)scaledY);

        for (int i = 1; i < maxHeight / 2 + 1; i++)
        {
            scaledY += i * gridSize;
            roadPos.y += i;
            if (tilemap5x5.GetTile(roadPos) == null)
                UnloadSidewalkAt(Mathf.CeilToInt(scaledX), Mathf.CeilToInt(scaledY));
            else
                tilemap5x5.SetTile(roadPos, null);
            UnloadDecorationAt(Mathf.CeilToInt(scaledX), Mathf.CeilToInt(scaledY));
            scaledY -= i * gridSize;
            roadPos.y -= i;
        }

        scaledY = pos.y * gridSize - gridSize / 2;
        for (int i = 1; i < maxHeight / 2 + 1; i++)
        {
            scaledY -= i * gridSize;
            roadPos.y -= i;
            if (tilemap5x5.GetTile(roadPos) == null)
                UnloadSidewalkAt(Mathf.CeilToInt(scaledX), Mathf.CeilToInt(scaledY));
            else
                tilemap5x5.SetTile(roadPos, null);
            UnloadDecorationAt(Mathf.CeilToInt(scaledX), Mathf.CeilToInt(scaledY));
            scaledY += i * gridSize;
            roadPos.y += i;
        }
    }
    private void UnloadSidewalkAt(int x, int y)
    {
        for (int offsetX = 0; offsetX < gridSize; offsetX++)
        {
            for (int offsetY = 0; offsetY < gridSize; offsetY++)
            {
                Vector3Int sidewalkPos = new(x + offsetX, y + offsetY, sidewalkZLevel);
                tilemap1x1.SetTile(sidewalkPos, null);
            }
        }
    }
    private void UnloadDecorationAt(int x, int y)
    {
        for (int offsetX = 0; offsetX < gridSize; offsetX++)
        {
            for (int offsetY = 0; offsetY < gridSize; offsetY++)
            {
                Vector3Int decorationPos = new(x + offsetX, y + offsetY, decorationZLevel);
                tilemap1x1.SetTile(decorationPos, null);
            }
        }
    }

    private void GenerateVerticalTiles(RoadTile tileData, int x, int y, GenerateDirection direction)
    {
        System.Random random = new System.Random(seed + y);

        RoadTile newTileData = tileData;
        for (int i = 1; i < maxHeight / 2 + 1; i++)
        {
            RoadEntry[] tileEntries = newTileData.possibleNorthTiles;
            if (tileEntries == null || tileEntries.Length == 0)
            {
                float scaledX = x * gridSize - gridSize / 2;
                float scaledY = (y + i) * gridSize - gridSize / 2;
                GenerateSidewalks(Mathf.CeilToInt(scaledX), Mathf.CeilToInt(scaledY));
                if (direction == GenerateDirection.LEFT)
                    structureGenerator.GenerateStructuresLeft(seed, new Vector2(scaledX, scaledY));
                else
                    structureGenerator.GenerateStructuresRight(seed, new Vector2(scaledX, scaledY));
                continue;
            }
            int newYPos = y + i;
            RoadEntry randomTileEntry = GetRandomRoadEntry(tileEntries, random);
            newTileData = randomTileEntry.tileData;
            SetRoadTile(newTileData, x, newYPos);
        }

        newTileData = tileData;
        for (int i = 1; i < maxHeight / 2 + 1; i++)
        {
            RoadEntry[] tileEntries = newTileData.possibleSouthTiles;
            if (tileEntries == null || tileEntries.Length == 0)
            {
                float scaledX = x * gridSize - gridSize / 2;
                float scaledY = (y - i) * gridSize - gridSize / 2;
                GenerateSidewalks(Mathf.CeilToInt(scaledX), Mathf.CeilToInt(scaledY));
                if (direction == GenerateDirection.LEFT)
                    structureGenerator.GenerateStructuresLeft(seed, new Vector2(scaledX, scaledY));
                else
                    structureGenerator.GenerateStructuresRight(seed, new Vector2(scaledX, scaledY));
                continue;
            }
            int newYPos = y - i;
            RoadEntry randomTileEntry = GetRandomRoadEntry(tileEntries, random);
            newTileData = randomTileEntry.tileData;
            SetRoadTile(newTileData, x, newYPos);
        }
    }
    private RoadEntry GetRandomRoadEntry(RoadEntry[] tileEntries, System.Random random)
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

    public void SetRoadTile(RoadTile tileData, int x, int y)
    {
        Vector3Int position = new(x, y, 0);
        float scaledX = x * gridSize - gridSize / 2;
        float scaledY = y * gridSize - gridSize / 2;
        tilemap5x5.SetTile(position, tileData);
        GenerateRoadDecorations((int)scaledX, (int)scaledY);
    }
    public void GenerateRoadDecorations(int x, int y)
    {
        System.Random random = new(seed + x + y);
        for (int offsetX = 0; offsetX < gridSize; offsetX++)
        {
            for (int offsetY = 0; offsetY < gridSize; offsetY++)
            {
                double chance = random.NextDouble();
                if (chance < 0.5)
                    continue;
                TileBase decoTile = GetRandomRoadDecoration(random);
                Vector3Int tilePos = new(x + offsetX, y + offsetY, decorationZLevel);
                Matrix4x4 matrix = Matrix4x4.TRS(new Vector3((float)random.NextDouble(), (float)random.NextDouble(), 0), Quaternion.Euler(0f, 0f, 0f), Vector3.one);
                tilemap1x1.SetTile(tilePos, decoTile);
                tilemap1x1.SetTransformMatrix(tilePos, matrix);
            }
        }
    }
    public TileBase GetRandomRoadDecoration(System.Random random)
    {
        int totalWeight = 0;
        foreach (DecorationEntry entry in roadDecorations)
        {
            totalWeight += entry.weight;
        }

        foreach (DecorationEntry entry in roadDecorations)
        {
            int randomInt = random.Next(totalWeight);
            if (randomInt < entry.weight)
                return entry.tile;
            totalWeight -= entry.weight;
        }
        return null;
    }

    public void GenerateSidewalks(int x, int y)
    {
        for (int offsetX = 0; offsetX < gridSize; offsetX++)
        {
            for (int offsetY = 0; offsetY < gridSize; offsetY++)
            {
                TileBase sideWalkTile = GetRandomSidewalk();
                Vector3Int tilePos = new(x + offsetX, y + offsetY, sidewalkZLevel);
                tilemap1x1.SetTile(tilePos, sideWalkTile);
            }
        }
        GenerateSidewalkDecorations(x, y);
    }
    public void GenerateSidewalkDecorations(int x, int y)
    {
        System.Random random = new(seed + x + y);
        for (int offsetX = 0; offsetX < gridSize; offsetX++)
        {
            for (int offsetY = 0; offsetY < gridSize; offsetY++)
            {
                double chance = random.NextDouble();
                if (chance < 0.5)
                    continue;
                TileBase decoTile = GetRandomSidewalkDecoration(random);
                Vector3Int tilePos = new(x + offsetX, y + offsetY, decorationZLevel);
                Matrix4x4 matrix = Matrix4x4.TRS(new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble()), Quaternion.Euler(0f, 0f, 0f), Vector3.one);
                tilemap1x1.SetTile(tilePos, decoTile);
                tilemap1x1.SetTransformMatrix(tilePos, matrix);
            }
        }
    }
    public TileBase GetRandomSidewalkDecoration(System.Random random)
    {
        int totalWeight = 0;
        foreach (DecorationEntry entry in sidewalkDecorations)
        {
            totalWeight += entry.weight;
        }

        foreach (DecorationEntry entry in sidewalkDecorations)
        {
            int randomInt = random.Next(totalWeight);
            if (randomInt < entry.weight)
                return entry.tile;
            totalWeight -= entry.weight;
        }
        return null;
    }

    public TileBase GetRandomSidewalk()
    {
        System.Random random = new System.Random();
        return sidewalks[random.Next(sidewalks.Length)];
    }
}

[Serializable]
public class DecorationEntry
{
    public TileBase tile;
    public int weight;
}

public enum GenerateDirection
{
    LEFT,
    RIGHT
}