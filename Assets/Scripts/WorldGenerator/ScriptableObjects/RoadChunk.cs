using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "RoadChunk", menuName = "Scriptable Objects/WorldGenerator/Sprite2D")]
[Serializable]
public class RoadChunk : ScriptableObject
{
    [SerializeField] IntVariable chunkSize;
    public SpriteGrid Sprites;
    public AdjacentRoad roadDirection;
}

[Serializable]
public struct AdjacentRoad
{
    public RoadChunk[] North;
    public RoadChunk[] South;
    public RoadChunk[] East;
    public RoadChunk[] West;
}

[Serializable]
public struct SpriteGrid
{
    public int RowNum;
    public int ColNum;  

    public float PreviewWidth;
    public float PreviewHeight;
    public float PreviewSpace;
    
    [Serializable]
    public struct SpriteColumn
    {
        public Sprite[] Columns;
    }
    public SpriteColumn[] Rows;
}

