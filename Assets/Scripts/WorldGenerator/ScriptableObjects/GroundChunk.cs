using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "RoadChunk", menuName = "Scriptable Objects/WorldGenerator/Sprite2D")]
[Serializable]
public class GroundChunk : ScriptableObject
{
    public GroundGenerator.GroundType Type;
    public SpriteGrid Sprites;
    public AdjacentRoad adjacentRoads;
}

[Serializable]
public struct AdjacentRoad
{
    public GroundChunk[] North;
    public GroundChunk[] South;
    public GroundChunk[] East;
    public GroundChunk[] West;
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

