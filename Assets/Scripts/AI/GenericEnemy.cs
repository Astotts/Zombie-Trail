using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GenericEnemy : NetworkBehaviour
{

    //Unit Movement
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float moveSpeed;
    [SerializeField] Rigidbody2D unitRB;

    //Tracking Destination
    [SerializeField] GetClosestTargets targetFinder;
    private Transform target;
    [SerializeField] WeaponsClass weapon;

    //Animation
    [SerializeField] Animator animator;
    private Vector2 lastPos;

    public override void OnNetworkSpawn()
    {
        if (!IsHost)
        {
            enabled = false;
            return;
        }
        base.OnNetworkSpawn();
    }
}
