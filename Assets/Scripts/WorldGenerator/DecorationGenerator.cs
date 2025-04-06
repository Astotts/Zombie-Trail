using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "DecorationGenerator", menuName = "Scriptable Objects/WorldGenerator/DecorationGenerator")]
public class DecorationGenerator : ChunkGenerator
{
    [SerializeField] [Range(0, 1)] private float spawnRate;
    [SerializeField] private int tileHeight;
    [SerializeField] private List<WeightedDecoration> decorationList;
    // [SerializeField] private List<Tile> tileToAdd;
    // [ContextMenu("Add Tiles")]
    // void AddTileTo()
    // {
    //     foreach (Tile tile in tileToAdd)
    //     {
    //         decorationList.Add(new WeightedDecoration { Weight = 1, Tile = tile});
    //     }
    // }
    
    private readonly Dictionary<Vector2Int, Dictionary<Vector3Int, Tile>> decorationData = new();
    private int totalWeight;

    void OnEnable()
    {
        totalWeight = 0;
        foreach (WeightedDecoration decoration in decorationList)
        {
            totalWeight += decoration.Weight;
        }
    }

    public override void OnChunkLoad(int seed, Vector2Int chunkPos, Tilemap tilemap, Dictionary<string, object> currentData)
    {
        if (decorationData.TryGetValue(chunkPos, out Dictionary<Vector3Int, Tile> chunkData))
        {
            LoadOldChunks(tilemap, chunkData);
        }
        else
        {
            GenerateNewDecorationChunk(seed, chunkPos, tilemap);
        }
    }

    void LoadOldChunks(Tilemap tilemap, Dictionary<Vector3Int, Tile> chunkData)
    {
        foreach (Vector3Int tilePos in chunkData.Keys)
        {
            tilemap.SetTile(tilePos, chunkData[tilePos]);
        }
    }

    void GenerateNewDecorationChunk(int seed, Vector2Int chunkPos, Tilemap tilemap)
    {
        UnityEngine.Random.InitState(seed + chunkPos.x + chunkPos.y * ChunkSize.Value);
        Vector2 worldPos = chunkPos * ChunkSize.Value;
        Dictionary<Vector3Int, Tile> generatedTile = new();
        for (int y = 0; y < ChunkSize.Value; y++)
        {
            for (int x = 0; x < ChunkSize.Value; x++)
            {
                float roll = UnityEngine.Random.Range(0f, 1f);
                if (roll > spawnRate)
                    continue;

                Vector3Int tilePos = new((int)(worldPos.x + x), (int)(worldPos.y + y), tileHeight);
                Tile randomTile = GetRandomDecorationTile();
                generatedTile.Add(tilePos, randomTile);
                tilemap.SetTile(tilePos, randomTile);
            }
        }
        decorationData.Add(chunkPos, generatedTile);
    }

    public override void OnChunkUnload(int seed, Vector2Int chunkPos, Tilemap tilemap, Dictionary<string, object> currentData)
    {
        if (!decorationData.TryGetValue(chunkPos, out Dictionary<Vector3Int, Tile> chunkData))
            return;
        
        foreach (Vector3Int tilePos in chunkData.Keys)
        {
            tilemap.SetTile(tilePos, null);
        }
    }

    Tile GetRandomDecorationTile()
    {
        int targetWeight = UnityEngine.Random.Range(0, totalWeight);
        int currentWeight = 0;
        foreach (WeightedDecoration decoration in decorationList)
        {
            if (currentWeight >= targetWeight)
                return decoration.Tile;
            currentWeight += decoration.Weight;
        }

        // This should never happen
        return null;
    }
}

[Serializable]
public class WeightedDecoration
{
    public int Weight;
    public Tile Tile;
}

[CustomPropertyDrawer(typeof(WeightedDecoration))]
public class DecorationInspector : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        SerializedProperty prefabProperty = property.FindPropertyRelative("Tile");
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        if (prefabProperty.objectReferenceValue != null)
            label.text = prefabProperty.objectReferenceValue.name;
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var weightRect = new Rect(position.x, position.y, position.width / 4 - 5, position.height);
        var prefabRect = new Rect(position.x + position.width / 4, position.y, position.width / 4 * 3, position.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(weightRect, property.FindPropertyRelative("Weight"), GUIContent.none);
        EditorGUI.PropertyField(prefabRect, prefabProperty, GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}