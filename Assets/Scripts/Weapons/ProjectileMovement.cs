using Unity.Netcode;
using Unity.Networking.Transport;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ProjectileMovement : NetworkBehaviour
{
    [SerializeField] GameObject bloodFX;                    // Blood Splash sprites on screen
    //[SerializeField] GameObject bulletTrail;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] NetworkObject networkObject;   // Assigned networkObject so this bullet can return to pool
    private GameObject prefab;                              // Assigned prefab so this bullet can return to pool
    private int penetration;                                // Ammount of penetration left before disappear
    private float speed;                                    // Current move speed
    private Quaternion rotation;                            // Current direction to travel
    private Vector2 initialPosition;                        // Initial Position
    public float range;                                     // Maximum range the projectile can travel
    public int damage;                                      // Damage to deal

    ParticleSystem bloodParticleSystem;

    public override void OnNetworkSpawn()
    {
        trailRenderer.Clear();
        if (!IsServer)
            return;
        initialPosition = this.transform.position;
        base.OnNetworkSpawn();
    }

    void SetupDirection(Quaternion rotation, float accuracy)
    {
        // rotation angle in degrees
        // rotation of the object
        float marginOfError = Random.Range(-100 / accuracy, 100 / accuracy);
        Quaternion initialRotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z - 90 + marginOfError);
        this.transform.rotation = initialRotation;
        // SetDirectionClientRpc(initialRotation);
    }

    // [Rpc(SendTo.ClientsAndHost)]
    // void SetDirectionClientRpc(Quaternion rotation)
    // {
    //     transform.rotation = rotation;
    // }

    void Update()
    {
        // since projectile was already rotated to face where mouse clicked all I have to do is shoot it.
        this.transform.position += speed * Time.deltaTime * this.transform.up;

        // Out of bounds code
        float distanceTraveled = Vector2.Distance(initialPosition, this.transform.position);
        
        if (!IsServer) return;

        if (distanceTraveled >= range || penetration < 1)
        {
            // Destroy the projectile when it goes out of range or penetrated enough zombies
            DestroySelfServerRpc();
        }
    }

    public void IntializeInfo(GameObject prefab, Quaternion rotation, int damage, int penetration, float accuracy, float speed, float range)
    {
        this.prefab = prefab;
        this.rotation = rotation;
        this.damage = damage;
        this.penetration = penetration;
        this.speed = speed;
        this.range = range;

        SetupDirection(rotation, accuracy);
    }

    [Rpc(SendTo.Server)]
    void DestroySelfServerRpc()
    {
        // Return to its network pool (Probably Destroyed, need to check this soon)
        NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject, prefab);
        networkObject.Despawn(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer)
            return;
        if (other.gameObject.CompareTag("Enemy") && this.CompareTag("Player bullet"))
        {
            if (other.gameObject.transform.parent != null)
            {
                if (other.gameObject.transform.parent.gameObject.TryGetComponent<IDamageable>(out var healthSystem))
                {
                    healthSystem.Damage(damage);
                    penetration--;
                }
            }
            else
            {
                other.gameObject.GetComponent<IDamageable>().Damage(damage);
                penetration--;
            }
            bloodFX = Instantiate(bloodFX);
            bloodFX.transform.localEulerAngles = new Vector3(transform.localEulerAngles.z - 90, bloodFX.transform.localEulerAngles.y, bloodFX.transform.localEulerAngles.z);
            bloodFX.transform.position = transform.position;
            bloodParticleSystem = bloodFX.GetComponent<ParticleSystem>();
            bloodParticleSystem.Play();
        }
        else if (other.CompareTag("Structure"))
        {
            float initialY = initialPosition.y;
            float buildingY = other.transform.position.y;
            if (initialY < buildingY || initialY > buildingY + other.GetComponent<SpriteRenderer>().size.y)
                return;

            DestroySelfServerRpc();
        }
    }
}
