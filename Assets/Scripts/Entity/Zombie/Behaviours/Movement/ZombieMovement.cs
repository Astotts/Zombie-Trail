using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class ZombieMovement : MonoBehaviour, IKnockable
{
    [SerializeField] Transform eyeDirection;
    [SerializeField] Rigidbody2D rb2D;
    [SerializeField] Animator animator;
    [SerializeField] BaseZombieMovementStats stats;

    public void MoveForward()
    {
        Vector2 direction = eyeDirection.rotation * Vector2.right;
        Vector2 displacement = stats.MoveSpeed * direction;

        animator.SetFloat("X", displacement.x);
        animator.SetFloat("Y", displacement.y);

        rb2D.AddForce(displacement);
    }

    public void Knock(Vector2 force)
    {
        rb2D.AddForce(force, ForceMode2D.Impulse);
    }
}