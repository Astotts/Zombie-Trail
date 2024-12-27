using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Structure : NetworkBehaviour
{
    [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
    [field: SerializeField] public BoxCollider2D BoxCollider2D { get; private set; }

    void OnValidate()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D = GetComponent<BoxCollider2D>();
    }

    public void SetStructureData(StructureSO structureSO, Vector2 spawnLocation, Quaternion spawnRotation)
    {
        SpriteRenderer.sprite = structureSO.Sprite;
        BoxCollider2D.offset = structureSO.ColliderOffset;
        BoxCollider2D.size = structureSO.ColliderSize;
        transform.SetPositionAndRotation(spawnLocation, spawnRotation);
    }
}
