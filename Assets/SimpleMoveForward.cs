using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMoveForward : MonoBehaviour
{
    private float speed = 0.25f;

    public void Update(){
        transform.position = new Vector3((transform.position.x + speed * Time.deltaTime), transform.position.y, transform.position.z);
        //speed += Time.deltaTime * 0.005f;
    }
}
