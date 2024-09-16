using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StructureDestroyOnCollide : MonoBehaviour
{
    // void OnCollisionEnter2D(Collision2D collision2D)
    // {
    //     Destroy(this.gameObject);
    //     Debug.Log("Collided");
    // }
    void OnTriggerEnter2D(Collider2D collider2D)
    {
        Destroy(this.gameObject);
        Debug.Log("Collided trigger");
    }
}
