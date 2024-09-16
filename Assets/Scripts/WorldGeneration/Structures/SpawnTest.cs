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
        for (int i = 0; i < 10; i++)
        {
            GameObject randomStructure = prefab;
            SpawnStructure(randomStructure, x, y);
            Debug.Log(prefab.GetComponent<BoxCollider2D>().size.x);
            x += prefab.GetComponent<BoxCollider2D>().size.x * prefab.transform.localScale.x;
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


    public void SpawnStructure(GameObject prefab, float offsetX, float offsetY)
    {
        float xOffset = 0.5f;
        float yOffset = 1.5f;
        Vector2 spawnLocation = new(offsetX + xOffset, offsetY + yOffset);
        GameObject go = Instantiate(prefab);
        go.transform.position = spawnLocation;
        gameObjects.Add(go);
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
