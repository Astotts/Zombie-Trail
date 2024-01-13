using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritePositioner : MonoBehaviour
{
    [SerializeField] Transform icon;
    [SerializeField] Transform parent;
    [SerializeField] Vector2 offset;

    // Update is called once per frame
    void Update()
    {
        icon.position = (Vector2)parent.position + offset;
        icon.rotation = Quaternion.Euler(Vector3.zero);
    }
}
