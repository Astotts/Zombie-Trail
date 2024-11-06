using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class CarMovement : NetworkBehaviour
{
    [SerializeField] Rigidbody2D rigid2D;

    float speed;


    void FixedUpdate()
    {
        Move(Vector2.right, speed * Time.deltaTime);
    }

    void Move(Vector3 direction, float speed)
    {
        rigid2D.MovePosition(rigid2D.transform.position + direction * speed);
    }

    public void SetSpeed(int amount)
    {
        speed = (float)amount / 10;
    }

    public void OnCollisionStay(Collision collision)
    {
        Debug.Log("Colliding");
    }
}