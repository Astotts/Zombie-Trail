using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    private float speed;
    private Vector3 direction;

    private Vector2 initialPosition;
    public float outOfBounds; // Maximum range the projectile can travel

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = this.transform.position; // Store the initial position of the projectile
    }

    public void InitiateMovement(Vector3 dir, float spe)
    {
        speed = spe;
        direction = dir;

        // rotation angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // rotation of the object
        this.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    }

    void Update()
    {
        // since projectile was already rotated to face where mouse clicked all I have to do is shoot it.
        this.transform.position += this.transform.right * speed * Time.deltaTime;

        // Out of bounds code
        float distanceTraveled = Vector2.Distance(initialPosition, this.transform.position);
        if (distanceTraveled >= outOfBounds)
        {
            Destroy(gameObject); // Destroy the projectile when it goes out of range
        }
    }
}
