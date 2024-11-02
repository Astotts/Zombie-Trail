using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class CarMovement : NetworkBehaviour
{
    [SerializeField] Rigidbody2D rigid2D;

    int speed;


    void FixedUpdate()
    {
        Move(Vector2.right, speed * Time.fixedDeltaTime);
    }

    void Move(Vector2 direction, float speed)
    {
        rigid2D.MovePosition((Vector2)transform.position + direction * speed);
    }

    public void SetSpeed(int amount)
    {
        speed = amount;
    }
}