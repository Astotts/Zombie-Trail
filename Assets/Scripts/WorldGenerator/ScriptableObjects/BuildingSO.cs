using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/WorldGenerator/Building")]
public class BuildingSO : ScriptableObject
{
    [field: SerializeField] public Sprite BuildingSprite { get; private set; }
    [field: SerializeField] public Vector2 ColliderCenter { get; private set; }
    [field: SerializeField] public Vector2 ColliderSize { get; private set; }

}