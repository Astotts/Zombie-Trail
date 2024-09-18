using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Road Tile", fileName = "New Road Tile")]
public class RoadTile : TileBase
{
    public Sprite sprite;
    public TileFlags flags;
    public Tile.ColliderType colliderType;
    public RoadEntry[] possibleNorthTiles;
    public RoadEntry[] possibleSouthTiles;
    public RoadEntry[] possibleWestTiles;
    public RoadEntry[] possibleEastTiles;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = sprite;
        tileData.colliderType = colliderType;
        tileData.flags = flags;
    }
}

[System.Serializable]
public class RoadEntry
{
    public RoadTile tileData;
    // More weight = higher chance to spawn
    public int weight;
}
