using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

public class Thrower : MonoBehaviour
{
    [SerializeField] float timePerThrow = 1f;
    [SerializeField] float projectileSpeed = 10;
    [SerializeField] float projectileHeight = 10;
    [SerializeField] Vector2 targetPos;
    [SerializeField] Vector3 spawnOffset;
    [SerializeField] GameObject projectile;
    [SerializeField] ZombieThrowAttackStats stats;
    float time;

    void Awake()
    {
        time = timePerThrow;
    }
    // Update is called once per frame
    void Update()
    {
        if (time > 0)
            time -= Time.deltaTime;
        else
        {
            GameObject gameObject = Instantiate(projectile, transform.position + spawnOffset, Quaternion.identity);

            ThrownProjectileMovement movement = gameObject.GetComponent<ThrownProjectileMovement>();

            movement.InitializeInfo(stats.ProjectileStats, targetPos);

            time = timePerThrow;
        }
    }
}
