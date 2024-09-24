using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Road Tile", fileName = "New Road Tile")]
public class RoadTile : TileBase
{
    [SerializeField] Sprite sprite;
    [SerializeField] TileFlags flags;
    [SerializeField] Tile.ColliderType colliderType;
    [SerializeField] RoadType roadType;
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
    public RoadType GetRoadType()
    {
        return roadType;
    }
}

[System.Serializable]
public class RoadEntry
{
    public RoadTile tileData;
    // More weight = higher chance to spawn
    public int weight;
}


public enum RoadType
{
    CROSS_INTERSECTION,
    ROAD_HORIZONTAL,
    ROAD_VERTICAL,
    T_INTERSECTION_UP,
    T_INTERSECTION_DOWN,
    CROSSWALK_NORTH,
    CROSSWALK_SOUTH,
    CROSSWALK_EAST,
    CROSSWALK_WEST,
    NONE
}