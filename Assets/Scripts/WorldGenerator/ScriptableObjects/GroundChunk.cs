using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Ground", menuName = "Scriptable Objects/WorldGenerator/Ground")]
[Serializable]
public class Ground : ScriptableObject
{
    public GroundGenerator.GroundType Type;
    public SpriteGrid Sprites;
    public AdjacentRoad adjacentRoads;
}

[Serializable]
public struct AdjacentRoad
{
    public Ground[] North;
    public Ground[] South;
    public Ground[] East;
    public Ground[] West;
}

[Serializable]
public struct SpriteGrid
{
    public int RowNum;
    public int ColNum;  

    public float PreviewSpace;
    
    [Serializable]
    public struct SpriteColumn
    {
        public Sprite[] Columns;
    }
    public SpriteColumn[] Rows;
}

