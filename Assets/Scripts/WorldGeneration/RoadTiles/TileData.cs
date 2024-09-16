using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "TileData", fileName = "New TileData")]
public class TileData : TileBase
{
    public Sprite sprite;
    public TileFlags flags;
    public Tile.ColliderType colliderType;
    public TileEntry[] possibleNorthTiles;
    public TileEntry[] possibleSouthTiles;
    public TileEntry[] possibleWestTiles;
    public TileEntry[] possibleEastTiles;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref UnityEngine.Tilemaps.TileData tileData)
    {
        tileData.sprite = sprite;
        tileData.flags = flags;
        tileData.colliderType = colliderType;
    }
}

[System.Serializable]
public class TileEntry
{
    public TileData tileData;
    // More weight = higher chance to spawn
    public int weight;
}
