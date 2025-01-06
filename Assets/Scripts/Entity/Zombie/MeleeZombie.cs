using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Tools.NetStatsMonitor;
using Unity.Netcode;
using Unity.Services.Relay.Scheduler;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeZombie : NetworkBehaviour, IZombie
{
    [field: SerializeField] public ZombieStats Stats { get; private set; }
    public Transform Target { get; private set; }
    public bool CanAttack => Target != null && Vector2.Distance(Target.position, transform.position) < Stats.AttackRange;
    public EZombieType Type => EZombieType.Melee;


    [SerializeField] NetworkObject networkObject;
    [SerializeField] ZombieStateMachine stateManager;
    [SerializeField] ZombieHealthDisplay healthDisplay;
    [SerializeField] ZombieMeleeAttack meleeAttack;
    [SerializeField] Rigidbody2D rb2D;

    void OnValidate()
    {
        if (networkObject == null)
            networkObject = GetComponent<NetworkObject>();
    }

    void Awake()
    {
        stateManager.Init(this);
        healthDisplay.Init(this);
        meleeAttack.Init(this);
    }

    public void Attack()
    {
        meleeAttack.Attack();
    }

    public bool FindTarget()
    {
        // IReadOnlyList<NetworkClient> playerList = NetworkManager.Singleton.ConnectedClientsList;
        List<GameObject> playerList = GameObject.FindGameObjectsWithTag("Player").ToList();

        float closestDistance = float.MaxValue;

        Target = null;

        foreach (GameObject player in playerList)
        {
            Vector2 playerPos = player.transform.position;
            float distance = Vector2.Distance(transform.position, playerPos);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                Target = player.transform;
                return true;
            }
        }
        return false;
    }

    public bool MoveTowardTarget()
    {
        float distanceToTarget = Vector2.Distance(Target.position, transform.position);
        if (distanceToTarget < Stats.AttackRange)
            return false;

        Vector2 direction = meleeAttack.transform.rotation * Vector2.right;
        rb2D.MovePosition(rb2D.position + direction * Stats.MoveSpeed * Time.deltaTime);
        return true;
    }
}
