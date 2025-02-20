using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Structure : MonoBehaviour
{
    [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
    [field: SerializeField] public BoxCollider2D BoxCollider2D { get; private set; }

    private List<StructureData> structureDataList;
    private StructureGenerator structureGenerator;
    private StructureData structureData;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Road"))
        {
            structureDataList.Remove(structureData);
            structureGenerator.ReturnStructure(this);
        }
    }

    void OnValidate()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D = GetComponent<BoxCollider2D>();
    }

    public void SetStructureData(StructureSO structureSO)
    {
        SpriteRenderer.sprite = structureSO.Sprite;
        BoxCollider2D.offset = structureSO.ColliderOffset;
        BoxCollider2D.size = structureSO.ColliderSize;
    }

    public void SetStructureList(List<StructureData> structureDataList, StructureData structureData)
    {
        this.structureDataList = structureDataList;
        this.structureData = structureData;
    }

    public void SetStructureGenerator(StructureGenerator structureGenerator)
    {
        this.structureGenerator = structureGenerator;
    }
}
