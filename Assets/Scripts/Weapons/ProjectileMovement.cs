using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileMovement : NetworkBehaviour
{
    private float speed;
    private Vector3 direction;
    private Vector2 initialPosition;
    public float outOfBounds; // Maximum range the projectile can travel
    public int damage;

    [SerializeField] GameObject bloodFX;
    //[SerializeField] GameObject bulletTrail;

    ParticleSystem bloodParticleSystem;
    TrailRenderer bulletTrailRenderer;

    public override void OnNetworkSpawn()
    {
        if (!IsHost)
        {
            enabled = false;
            return;
        }
        base.OnNetworkSpawn();
    }

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
            if (other.gameObject.transform.parent != null)
            {
                if (other.gameObject.transform.parent.gameObject.TryGetComponent<HealthSystem>(out var healthSystem))
                    healthSystem.AlterHealth(-damage);
            }
            else
            {
                other.gameObject.GetComponent<HealthSystem>().AlterHealth(-damage);
            }
            bloodFX = Instantiate(bloodFX);
            bloodFX.transform.localEulerAngles = new Vector3(transform.localEulerAngles.z - 90, bloodFX.transform.localEulerAngles.y, bloodFX.transform.localEulerAngles.z);
            bloodFX.transform.position = transform.position;
            bloodParticleSystem = bloodFX.GetComponent<ParticleSystem>();
            bloodParticleSystem.Play();
        }
        else if (other.CompareTag("Structure"))
        {
            Destroy(gameObject);
        }
    }
}
