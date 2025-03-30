using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable Objects/WorldGenerator/GroundGenerator", fileName = "New Ground Generator")]
public class GroundGenerator : ChunkGenerator
{
    public static readonly string GROUND_GENERATOR_DATA_ID = "GROUND_GEN_DATA";
    [SerializeField] private List<GroundChunk> groundList = new();
    [SerializeField] private List<Tile> tileBaseList = new();
    [SerializeField] private int groundHeight;

    private readonly Dictionary<GroundType, PossibleGrounds> possibleGroundMap = new();
    private readonly Dictionary<GroundType, Dictionary<Vector2Int, TileBase>> groundSpriteGridMap = new();

    void OnEnable()
    {
        foreach (GroundChunk groundChunk in groundList)
        {
            PossibleGrounds possibleGrounds = new()
            {
                North = GetGroundID(groundChunk.adjacentRoads.North),
                South = GetGroundID(groundChunk.adjacentRoads.South),
                East = GetGroundID(groundChunk.adjacentRoads.East),
                West = GetGroundID(groundChunk.adjacentRoads.West),
            };

            possibleGroundMap.Add(groundChunk.Type, possibleGrounds);


            SpriteGrid.SpriteColumn[] rows = groundChunk.Sprites.Rows;
            Dictionary<Vector2Int, TileBase> spriteMap = new();

            for (int y = rows.Length - 1; y >= 0; y--)
            {
                for (int x = rows[y].Columns.Length - 1; x >= 0; x--)
                {
                    Vector2Int pos = new(x, -y);
                    foreach (Tile tile in tileBaseList)
                    {
                        if (tile.sprite != rows[y].Columns[x])
                            continue;
                        
                        spriteMap.Add(pos, tile);
                        break;
                    }
                }
            }

            groundSpriteGridMap.Add(groundChunk.Type, spriteMap);
        }
    }

    HashSet<GroundType> GetGroundID(GroundChunk[] possibleGround)
    {
        HashSet<GroundType> idList = new();
        foreach (GroundChunk ground in possibleGround)
        {
            idList.Add(ground.Type);
        }
        return idList;
    }

    public override void OnChunkLoad(int seed, Vector2Int chunkPos, Tilemap tilemap, Dictionary<string, object> currentData)
    {
        GroundData groundData;
        if (currentData.TryGetValue(GROUND_GENERATOR_DATA_ID, out object data))
        {
            groundData = (GroundData) data;
        }
        else
        {
            groundData = new()
            {
                groundMap = new()
            };
            currentData.Add(GROUND_GENERATOR_DATA_ID, groundData);
        }

        if (!groundData.groundMap.TryGetValue(chunkPos, out GroundType groundType))
        {
            groundType = GetRandomGroundType(seed, chunkPos, groundData.groundMap);
            groundData.groundMap[chunkPos] = groundType;
        }

        Dictionary<Vector2Int, TileBase> sprites = groundSpriteGridMap[groundType];
        Vector2Int worldPos = chunkPos * ChunkSize.Value;
        foreach (Vector2Int offset in sprites.Keys)
        {
            Vector2Int pos = worldPos + offset;
            tilemap.SetTile(new Vector3Int(pos.x, pos.y, groundHeight), sprites[offset]);
        }
    }

    GroundType GetRandomGroundType(int seed, Vector2Int chunkPos, Dictionary<Vector2Int, GroundType> groundMap) {
        Vector2Int northPos = new(chunkPos.x, chunkPos.y + 1);
        Vector2Int southPos = new(chunkPos.x, chunkPos.y - 1);
        Vector2Int eastPos = new(chunkPos.x + 1, chunkPos.y);
        Vector2Int westPos = new(chunkPos.x - 1, chunkPos.y);
        
        PossibleGrounds northPossible = null;
        if (groundMap.TryGetValue(northPos, out GroundType northGroundType))
        {
            northPossible = possibleGroundMap[northGroundType];
        }

        PossibleGrounds southPossible = null;
        if (groundMap.TryGetValue(southPos, out GroundType southGroundType))
        {
            southPossible = possibleGroundMap[southGroundType];
        }

        PossibleGrounds eastPossible = null;
        if (groundMap.TryGetValue(eastPos, out GroundType eastGroundType))
        {
            eastPossible = possibleGroundMap[eastGroundType];
        }

        PossibleGrounds westPossible = null;
        if (groundMap.TryGetValue(westPos, out GroundType westGroundType))
        {
            westPossible = possibleGroundMap[westGroundType];
        }

        if (northPossible == null
        && southPossible == null
        && eastPossible == null
        && westPossible == null)
        {
            return GroundType.INTERSECTION;
        }

        List<GroundType> possibleGroundType = new();
        foreach (GroundChunk groundChunk in groundList)
        {
            GroundType type = groundChunk.Type;

            if (
                (northPossible == null || northPossible.South.Contains(type))
                && (southPossible == null || southPossible.North.Contains(type))
                && (eastPossible == null || eastPossible.West.Contains(type))
                && (westPossible == null || westPossible.East.Contains(type))
            )
            {
                possibleGroundType.Add(type);
            }
        }

        Random.InitState(seed + chunkPos.x);
        int index = Random.Range(0, possibleGroundType.Count);

        return possibleGroundType[index];
    }

    public override void OnChunkUnload(int seed, Vector2Int chunkPos, Tilemap tilemap, Dictionary<string, object> currentData)
    {
        // Debug.Log("Unloading");
        if (!currentData.TryGetValue(GROUND_GENERATOR_DATA_ID, out object data))
            return;
        // Debug.Log("Found Grounddata");
        GroundData groundData = (GroundData) data;

        if (!groundData.groundMap.TryGetValue(chunkPos, out GroundType groundType))
            return;

        // Debug.Log("Found groundtype");
        Dictionary<Vector2Int, TileBase> sprites = groundSpriteGridMap[groundType];
        Vector2Int worldPos = chunkPos * ChunkSize.Value;

        foreach (Vector2Int offset in sprites.Keys)
        {
            Vector2Int pos = worldPos + offset;
            tilemap.SetTile(new Vector3Int(pos.x, pos.y, groundHeight), null);
        }
    }

    public class PossibleGrounds
    {
        public HashSet<GroundType> North;
        public HashSet<GroundType> South;
        public HashSet<GroundType> East;
        public HashSet<GroundType> West;
    }

    public struct GroundData
    {
        public Dictionary<Vector2Int, GroundType> groundMap;
    }

    public enum GroundType : byte
    {
        HORIZONTAL_ROAD,
        VERTICAL_ROAD,
        T_INTERSECTION_UP_ROAD,
        T_INTERSECTION_DOWN_ROAD,
        INTERSECTION,
        CROSSWALK_NORTH,
        CROSSWALK_SOUTH,
        CROSSWALK_EAST,
        CROSSWALK_WEST,
        PAVEMENT,
    }
}