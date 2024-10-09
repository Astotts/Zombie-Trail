using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class CarMovement : NetworkBehaviour
{
    [SerializeField] CarStats _carStats;
    [SerializeField] Rigidbody2D rigid2D;
    public CarStats Stats => _carStats;


    void FixedUpdate()
    {
        Move(Vector2.right, Stats.Speed * Time.fixedDeltaTime);
    }

    void Move(Vector2 direction, float speed)
    {
        rigid2D.MovePosition((Vector2)transform.position + direction * speed);
    }
}