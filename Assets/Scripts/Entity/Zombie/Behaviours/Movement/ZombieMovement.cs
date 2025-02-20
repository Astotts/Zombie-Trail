using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class ZombieMovement : MonoBehaviour, IKnockable
{
    public float MoveSpeed => stats.MoveSpeed;
    public float MoveAnimationSpeed => stats.MoveAnimationSpeed;

    [SerializeField] Transform eyeDirection;
    [SerializeField] Rigidbody2D rb2D;
    [SerializeField] BaseZombieMovementStats stats;

    public void MoveForward()
    {
        Vector2 direction = eyeDirection.rotation * Vector2.right;
        Vector2 displacement = stats.MoveSpeed * direction;

        rb2D.AddForce(displacement);
    }

    public void Knock(Vector2 force)
    {
        rb2D.AddForce(force, ForceMode2D.Impulse);
    }
}