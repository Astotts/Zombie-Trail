using Unity.Netcode;
using UnityEngine;

public class ProjectileMovement : NetworkBehaviour
{
    [SerializeField] GameObject bloodFX;                    // Blood Splash sprites on screen
    //[SerializeField] GameObject bulletTrail;
    [SerializeField] private NetworkObject networkObject;   // Assigned networkObject so this bullet can return to pool
    private GameObject prefab;                              // Assigned prefab so this bullet can return to pool
    private int currentPenetration;                         // Ammount of penetration left before disappear
    private float speed;                                    // Current move speed
    private Vector3 direction;                              // Current direction to travel
    private Vector2 initialPosition;                        // Initial Position
    public float outOfBounds;                               // Maximum range the projectile can travel
    public int damage;                                      // Damage to deal

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

    public void InitiateMovement(Vector3 direction, float speed, int damage)
    {
        this.direction = direction;
        this.damage = damage;
        this.speed = speed;

        // rotation angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // rotation of the object
        this.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    }

    void Update()
    {
        // since projectile was already rotated to face where mouse clicked all I have to do is shoot it.
        this.transform.position += speed * Time.deltaTime * this.transform.up;

        // Out of bounds code
        float distanceTraveled = Vector2.Distance(initialPosition, this.transform.position);
        if (distanceTraveled >= outOfBounds)
        {
            // Destroy the projectile when it goes out of range
            DestroySelf();
        }
    }

    void DestroySelf()
    {
        // Return to its network pool
        NetworkObjectPool.Singleton.ReturnNetworkObject(this.networkObject, this.gameObject);
        // Destroy and return to pool on client side
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && this.CompareTag("Player bullet"))
        {
            if (other.gameObject.transform.parent != null)
            {
                if (other.gameObject.transform.parent.gameObject.TryGetComponent<IDamageable>(out var healthSystem))
                    healthSystem.Damage(damage);
            }
            else
            {
                other.gameObject.GetComponent<IDamageable>().Damage(damage);
            }
            bloodFX = Instantiate(bloodFX);
            bloodFX.transform.localEulerAngles = new Vector3(transform.localEulerAngles.z - 90, bloodFX.transform.localEulerAngles.y, bloodFX.transform.localEulerAngles.z);
            bloodFX.transform.position = transform.position;
            bloodParticleSystem = bloodFX.GetComponent<ParticleSystem>();
            bloodParticleSystem.Play();
        }
        else if (other.CompareTag("Structure"))
        {
            DestroySelf();
        }
    }
}
