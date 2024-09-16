using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    public GameObject prefab;

    void Start()
    {
        BoxCollider2D boxCollider2D = prefab.GetComponent<BoxCollider2D>();
        Vector3 size = boxCollider2D.size * 5;
        Collider2D collider2D = Physics2D.OverlapBox(new Vector2(size.x + 0.5f, size.y / 2), size, 0);
        if (collider2D != null)
            Debug.Log("Collided");
    }

    void OnDrawGizmos()
    {
        BoxCollider2D boxCollider2D = prefab.GetComponent<BoxCollider2D>();
        Vector3 size = boxCollider2D.size * 5;
        Gizmos.DrawWireCube(new Vector3(size.x, size.y / 2, 0), boxCollider2D.size * 5);
    }
}
