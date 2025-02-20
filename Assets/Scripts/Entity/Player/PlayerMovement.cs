using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IKnockable
{
    [SerializeField] Rigidbody2D rb2D;
    [SerializeField] PlayerMoveStats stats;
    Vector2 velocity;

    public void Input(Vector2 input)
    {
        velocity = input * stats.MoveSpeed;
    }

    public void Knock(Vector2 force)
    {
        rb2D.AddForce(force, ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        Move(velocity);
    }

    void Move(Vector2 velocity)
    {
        rb2D.AddForce(velocity);
    }
}
