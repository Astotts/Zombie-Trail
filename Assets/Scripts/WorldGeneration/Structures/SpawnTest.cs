using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    public GameObject prefab;
    public BoxCollider2D collideDetector;
    public float x;
    public float y;
    private List<GameObject> gameObjects = new();

    public void Spawn()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            DestroyImmediate(gameObject);
        }
        // StartCoroutine(DelaySpawn());
        System.Random random = new();
        Vector2 chunkLocation = new Vector2(x, y);
        for (int i = 0; i < 100; i++)
        {
            float x = chunkLocation.x - i;
            float y = chunkLocation.y + random.Next(5 / 2);
            GameObject randomStructure = prefab;
            GameObject spawnedStructure = SpawnStructure(randomStructure, x, y);
            if (spawnedStructure != null)
            {
                BoxCollider2D boxCollider2D = randomStructure.GetComponent<BoxCollider2D>();
                float structureXLength = boxCollider2D.size.x * randomStructure.transform.localScale.x;
                i += (int)structureXLength - 1;
            }
        }
    }

    // public IEnumerator DelaySpawn()
    // {
    //     // System.Random random = new System.Random(0);
    //     // for (int i = 0; i < 10; i++)
    //     // {
    //     //     x = (int)(x + i);
    //     //     y = (int)(y);
    //     //     GameObject randomStructure = prefab;
    //     //     SpawnStructure(randomStructure, x, y);
    //     //     x -= i;
    //     //     yield return new WaitForSeconds(0.5f);
    //     // }
    // }


    public GameObject SpawnStructure(GameObject prefab, float offsetX, float offsetY)
    {
        float xOffset = 2;
        float yOffset = 2;
        Vector2 spawnLocation = new(offsetX + xOffset, offsetY + yOffset);
        BoxCollider2D boxCollider2D = prefab.GetComponent<BoxCollider2D>();
        Vector3 scale = boxCollider2D.transform.localScale;
        Vector3 size = boxCollider2D.size * scale;
        Vector2 sizeOffset = boxCollider2D.offset * scale;
        float sizeMultiplier = 0.98f;
        Collider2D hitCollider = Physics2D.OverlapBox(new Vector2(spawnLocation.x - sizeOffset.x, spawnLocation.y + sizeOffset.y), size * sizeMultiplier, 0);

        if (hitCollider != null)
        {
            return null;
        }

        GameObject structure = Instantiate(prefab, this.transform);
        structure.transform.SetPositionAndRotation(spawnLocation + new Vector2(boxCollider2D.size.x, 0), Quaternion.Euler(0, 180, 0));
        gameObjects.Add(structure);
        return structure;
    }

    void OnDrawGizmos()
    {
        float xOffset = 0.5f;
        float yOffset = 1.5f;
        Vector2 spawnLocation = new(x + xOffset, y + yOffset);
        BoxCollider2D boxCollider2D = prefab.GetComponent<BoxCollider2D>();
        Vector3 scale = boxCollider2D.transform.localScale;
        Vector3 size = boxCollider2D.size * scale;
        Vector2 sizeOffset = boxCollider2D.offset * scale;
        float sizeMultiplier = 0.98f;
        // Debug.Log(boxCollider2D.size + " * " + scale + " = " + size);
        Gizmos.DrawWireCube(spawnLocation + sizeOffset, size * sizeMultiplier);
    }
}

[CustomEditor(typeof(SpawnTest))]
public class SpawnTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SpawnTest spawnTest = (SpawnTest)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Spawn"))
        {
            spawnTest.Spawn();
        }
    }
}
