using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    private float speed;
    private Vector3 direction;
    private Vector2 initialPosition;
    public float outOfBounds; // Maximum range the projectile can travel
    private int damage;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = this.transform.position; // Store the initial position of the projectile
    }

    public void InitiateMovement(Vector3 dir, float spe, int dam)
    {
        speed = spe;
        direction = dir;
        damage = dam; 

        // rotation angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // rotation of the object
        this.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle - 90));
    }

    void Update()
    {
        // since projectile was already rotated to face where mouse clicked all I have to do is shoot it.
        this.transform.position += this.transform.up * speed * Time.deltaTime;

        // Out of bounds code
        float distanceTraveled = Vector2.Distance(initialPosition, this.transform.position);
        if (distanceTraveled >= outOfBounds)
        {
            Destroy(gameObject); // Destroy the projectile when it goes out of range
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && this.CompareTag("Player bullet"))
        {
            other.gameObject.GetComponent<HealthSystem>().AlterHealth(-damage);
        }
    }
}
