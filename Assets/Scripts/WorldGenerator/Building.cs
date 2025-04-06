
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
    [field: SerializeField] public BoxCollider2D BoxCollider2D { get; private set; }

    private BuildingGenerator buildingGenerator;
    private List<SpawnedBuilding> dataList;
    private SpawnedBuilding buildingData;

    public void RemoveSelfFromData()
    {
        dataList.Remove(buildingData);
        buildingGenerator.ReturnBuilding(this);
    }

    void OnValidate()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D = GetComponent<BoxCollider2D>();
    }


    public void SetBuildingData(BuildingSO buildingSO)
    {
        if (buildingSO == null)
            Debug.Log("Found you");
        SpriteRenderer.sprite = buildingSO.BuildingSprite;
        BoxCollider2D.offset = buildingSO.ColliderCenter;
        BoxCollider2D.size = buildingSO.ColliderSize;
    }

    public void SetBuildingList(List<SpawnedBuilding> buildingDataList, SpawnedBuilding buildingData)
    {
        this.dataList = buildingDataList;
        this.buildingData = buildingData;
    }

    public void SetBuildingGenerator(BuildingGenerator buildingGenerator)
    {
        this.buildingGenerator = buildingGenerator;
    }
}