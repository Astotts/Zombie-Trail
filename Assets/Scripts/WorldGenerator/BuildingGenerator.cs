using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "BuildingGenerator", menuName = "Scriptable Objects/WorldGenerator/BuildingGenerator")]
public class BuildingGenerator : ChunkGenerator
{
    public static readonly string BUILDING_GENERATOR_DATA_ID = "BUILDING_GEN_DATA";
    [SerializeField] int prewarmGOAmount = 15;
    [SerializeField] GameObject buildingPrefab;
    [SerializeField] private float lengthRatio;
    [SerializeField] private Vector2 buildingOffset;
    [SerializeField] private LayerMask buildingLayer;
    [SerializeField] private List<WeightedBuilding> frontBuildingPrefabs;
    [SerializeField] private List<WeightedBuilding> backBuildingPrefabs;

    private readonly Dictionary<Vector2Int, List<SpawnedBuilding>> buildingData = new();
    private readonly Stack<Building> inactiveBuildings = new();
    private int frontTotalWeight;
    private int backTotalWeight;
    
    void OnEnable()
    {
        frontTotalWeight = 0;
        foreach (WeightedBuilding building in frontBuildingPrefabs)
        {
            frontTotalWeight += building.weight;
        }

        backTotalWeight = 0;
        foreach (WeightedBuilding building in backBuildingPrefabs)
        {
            // Cache the total weight of building pool
            backTotalWeight += building.weight;
        }
    }

    Building SpawnBuildingGO()
    {
        GameObject inactiveBuilding = Instantiate(buildingPrefab);
        Building emptyBuilding = inactiveBuilding.GetComponent<Building>();
        emptyBuilding.SetBuildingGenerator(this);
        return emptyBuilding;
    }

    Building SpawnBuildingGO(Vector2 spawnPos, Quaternion spawnRot)
    {
        GameObject inactiveBuilding = Instantiate(buildingPrefab, spawnPos, spawnRot);
        Building emptyBuilding = inactiveBuilding.GetComponent<Building>();
        emptyBuilding.SetBuildingGenerator(this);
        return emptyBuilding;
    }

    Building GetBuilding(Vector2 spawnPos, Quaternion spawnRot)
    {
        if (inactiveBuildings.TryPop(out Building building))
        {
            building.transform.SetPositionAndRotation(spawnPos, spawnRot);
            building.gameObject.SetActive(true);
            return building;
        }

        return SpawnBuildingGO(spawnPos, spawnRot);
    }

    public void ReturnBuilding(Building building)
    {
        building.gameObject.SetActive(false);
        inactiveBuildings.Push(building);
    }

    public override void OnChunkLoad(int seed, Vector2Int chunkPos, Tilemap tilemap, Dictionary<string, object> currentData)
    {
        GroundGenerator.GroundData groundData;
        // Don't generate buildings if road data did not exist
        // Logically, it should only happen once
        if (!currentData.TryGetValue(GroundGenerator.GROUND_GENERATOR_DATA_ID, out object data))
            return;
        
        groundData = (GroundGenerator.GroundData) data;

        if (groundData.groundMap[chunkPos] != GroundGenerator.GroundType.PAVEMENT)
        {
            RemoveBuildingsAtChunk(chunkPos);
            return;
        }

        
        if (!currentData.ContainsKey(BUILDING_GENERATOR_DATA_ID))
            currentData.Add(BUILDING_GENERATOR_DATA_ID, buildingData);

        if (buildingData.TryGetValue(chunkPos, out List<SpawnedBuilding> buildingList))
        {
            foreach (SpawnedBuilding buildingData in buildingList)
            {
                Building building = GetBuilding(buildingData.SpawnPos, Quaternion.identity);
                buildingData.SpawnedGO = building;
            }
        }
        else
        {
            Vector2Int leftChunkPos = new(chunkPos.x - 1, chunkPos.y);
            buildingList = GenerateBuildings(seed, chunkPos * ChunkSize.Value, groundData.groundMap.ContainsKey(leftChunkPos), chunkPos.y >= 0);
            buildingData.Add(chunkPos, buildingList);
        }
    }

    List<SpawnedBuilding> GenerateBuildings(int seed, Vector2 worldPos, bool isRight, bool isFront)
    {
        List<SpawnedBuilding> spawnedBuildingList = new();
        for (float i = 0; i < ChunkSize.Value; i++)
        {
            BuildingSO randomBuilding;
            Vector2 buildingSize;
            Vector2 offset;
            if (isFront)
            {
                randomBuilding = GetRandomFrontBuilding(seed, worldPos);
                buildingSize = randomBuilding.ColliderSize;
                offset.x = isRight ? (buildingOffset.x + buildingSize.x / 2 + i) : (-buildingOffset.x - buildingSize.x / 2 - i + ChunkSize.Value);
                offset.y = buildingOffset.y;
            }
            else
            {
                randomBuilding = GetRandomBackBuilding(seed, worldPos);
                buildingSize = randomBuilding.ColliderSize;
                offset.x = isRight ? (buildingOffset.x + buildingSize.x / 2 + i) : (-buildingOffset.x - buildingSize.x / 2 - i + ChunkSize.Value);
                offset.y = -buildingOffset.y - buildingSize.y + ChunkSize.Value;
            }

            // Assuming the center of the building is actually at the center
            // Have to shift the coordinate by a little
            Vector2 spawnPos = worldPos + offset;
            
            if (IsBuildingSpawnable(spawnPos, buildingSize))
            {
                Building spawnedGO = GetBuilding(spawnPos, Quaternion.identity);
                spawnedGO.SetBuildingData(randomBuilding);
                spawnedGO.SetBuildingGenerator(this);
                SpawnedBuilding spawnedBuilding = new()
                {
                    BuildingSO = randomBuilding,
                    SpawnedGO = spawnedGO,
                    SpawnPos = spawnPos
                };
                spawnedBuildingList.Add(spawnedBuilding);
                spawnedGO.SetBuildingList(spawnedBuildingList, spawnedBuilding);
                
                i += buildingSize.x - 1;
            }
        }
        return spawnedBuildingList;
    }

    void RemoveBuildingsAtChunk(Vector2Int chunkPos)
    {
        Vector2 worldPos = chunkPos * ChunkSize.Value;
        Vector2 endPos = worldPos + Vector2.one * ChunkSize.Value;
        foreach (Collider2D collider2D in Physics2D.OverlapAreaAll(worldPos, endPos, buildingLayer))
        {
            Building building = collider2D.GetComponent<Building>();
            building.RemoveSelfFromData();
        }
    }

    public override void OnChunkUnload(int seed, Vector2Int chunkPos, Tilemap tilemap, Dictionary<string, object> currentData)
    {

    }

    bool IsBuildingSpawnable(Vector2 spawnPos, Vector2 buildingSize)
    {
        Vector2 boxPos = spawnPos;
        boxPos.y += buildingSize.y / 2;
        // Cast a box to see if the area is taken by other building
        return !Physics2D.OverlapBox(boxPos, buildingSize * 0.98f, 0, buildingLayer);
    }

    BuildingSO GetRandomFrontBuilding(int seed, Vector2 worldPos)
    {
        // Make sure the random is predeterministic
        UnityEngine.Random.InitState((int)(seed + worldPos.x + worldPos.y * ChunkSize.Value));
        int targetWeight = UnityEngine.Random.Range(0, frontTotalWeight);
        int currentWeight = 0;
        byte id = 0;
        foreach (WeightedBuilding building in frontBuildingPrefabs)
        {
            if (currentWeight >= targetWeight)
                return building.buildingSO;
            currentWeight += building.weight;
            id++;
        }

        // This should never happen
        return null;
    }
    BuildingSO GetRandomBackBuilding(int seed, Vector2 worldPos)
    {
        // Make sure the random is predeterministic
        UnityEngine.Random.InitState((int)(seed + worldPos.x + worldPos.y * ChunkSize.Value));
        int targetWeight = UnityEngine.Random.Range(0, backTotalWeight);
        int currentWeight = 0;

        byte id = 0;
        Debug.Log("Target: " + targetWeight + "/" + backTotalWeight);
        foreach (WeightedBuilding building in backBuildingPrefabs)
        {
            if (currentWeight >= targetWeight)
                return building.buildingSO;
            currentWeight += building.weight;
            id++;
        }

        // This should never happen
        return null;
    }
}
    
[Serializable]
public class WeightedBuilding
{
    public int weight;
    public BuildingSO buildingSO;
}

public struct BuildingData
{
    public Dictionary<Vector2Int, List<SpawnedBuilding>> SpawnedBuildingMap;
}
public class SpawnedBuilding
{
    public BuildingSO BuildingSO;
    public Building SpawnedGO;
    public Vector2 SpawnPos;
}