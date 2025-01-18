using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Networking.PlayerConnection;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class ZombieSpawner : NetworkBehaviour
{
    [SerializeField] ZombieSpawnerStatsSO stats;
    int currentPoolLevel;

    Bounds leashRange;
    Bounds spawnRange;

    Offsets offsets;

    Coroutine spawnLoop;
    Coroutine leashLoop;

    readonly List<NetworkObject> spawnedZombieList = new();
    readonly List<Rigidbody2D> playerRigidList = new();

    void OnDrawGizmos()
    {
        if (stats == null)
            return;

        foreach (Rigidbody2D rigid2D in playerRigidList)
        {
            float circleRadius = stats.DistanceBetweenOffsets / 2;
            foreach (Vector2 offset in offsets.AllOffsets)
            {
                Collider2D colliderHit = Physics2D.OverlapCircle(rigid2D.position + offset, circleRadius, stats.LayerWhitelist);

                if (!colliderHit)
                    Gizmos.color = Color.yellow;
                else
                    Gizmos.color = Color.green;

                Gizmos.DrawSphere(rigid2D.position + offset, circleRadius);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(rigid2D.position + stats.LeashAreaCenter, stats.LeashAreaExtends);
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(rigid2D.position + stats.SpawnAreaCenter, stats.SpawnAreaExtends);
        }
    }

    void Awake()
    {
        leashRange = new(stats.LeashAreaCenter, stats.LeashAreaExtends);
        spawnRange = new(stats.SpawnAreaCenter, stats.SpawnAreaExtends);

        offsets = new(spawnRange, stats.DistanceBetweenOffsets);
    }

    void Start()
    {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            playerRigidList.Add(client.PlayerObject.GetComponent<Rigidbody2D>());
        }
        PlayerManager.Instance.OnPlayerSpawned += OnPlayerJoined;
    }

    private void OnPlayerJoined(object sender, NetworkObject playerObject)
    {
        playerRigidList.Add(playerObject.GetComponent<Rigidbody2D>());
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            Destroy(this.gameObject);
            return;
        }
        WaveManager.Instance.OnStateChange += OnWaveStateChange;
    }

    void OnWaveStateChange(object sender, WaveState state)
    {
        if (state == WaveState.StartNight)
            StartSpawning(WaveManager.Instance.CurrentDay);
        else if (state == WaveState.EndNight)
            StopSpawning();
    }

    public void StartSpawning(int level)
    {
        currentPoolLevel = level;

        spawnLoop = StartCoroutine(SpawnLoop());
        leashLoop = StartCoroutine(LeashLoop());
    }

    public void StopSpawning()
    {
        StopCoroutine(spawnLoop);
        StopCoroutine(leashLoop);
    }

    public void SpawnRandomZombieAtPlayersFront()
    {
        if (spawnedZombieList.Count > stats.GetMaxZombie(WaveManager.Instance.CurrentDay))
            return;
        GameObject prefab = stats.GetRandomZombiePrefab(currentPoolLevel);

        foreach (Rigidbody2D rigid2D in playerRigidList)
        {
            Vector2 spawnPos = GetRandomFrontSpawnPos(rigid2D, prefab.GetComponent<CircleCollider2D>());

            SpawnZombie(prefab, spawnPos, Quaternion.identity);
        }
    }

    Vector2 GetRandomFrontSpawnPos(Rigidbody2D playerRigid2D, Collider2D zombieCollider)
    {
        Vector2 playerPos = playerRigid2D.position;
        Vector2 playerDirection = playerRigid2D.velocity.normalized;
        List<Vector2> spawnOffsets;
        List<Vector2> spawnLocations = new();

        if (playerDirection == Vector2.zero)
        {
            spawnOffsets = offsets.AllOffsets;
        }
        else if (Mathf.Abs(playerDirection.y) > Mathf.Abs(playerDirection.x))
        {
            if (playerDirection.y > 0)
                spawnOffsets = offsets.TopOffsets;
            else
                spawnOffsets = offsets.BottomOffsets;
        }
        else
        {
            if (playerDirection.x > 0)
                spawnOffsets = offsets.RightOffsets;
            else
                spawnOffsets = offsets.LeftOffsets;
        }

        foreach (Vector2 offset in spawnOffsets)
        {
            Vector2 spawnPos = playerPos + offset;
            Collider2D colliderHit = Physics2D.OverlapBox(spawnPos, zombieCollider.bounds.extents, 0, stats.LayerWhitelist);

            if (colliderHit)
                spawnLocations.Add(spawnPos);
        }

        if (spawnLocations.Count == 0)
        {
            foreach (Vector2 offset in offsets.AllOffsets)
            {
                Vector2 spawnPos = playerPos + offset;
                Collider2D colliderHit = Physics2D.OverlapBox(spawnPos, zombieCollider.bounds.extents, 0, stats.LayerWhitelist);

                if (colliderHit)
                    spawnLocations.Add(spawnPos);
            }
        }

        return spawnLocations[UnityEngine.Random.Range(0, spawnLocations.Count)];
    }

    IEnumerator SpawnLoop()
    {
        SpawnRandomZombieAtPlayersFront(); // Level + front calculation
        yield return new WaitForSeconds(stats.GetSecondsPerSpawn(currentPoolLevel));
        spawnLoop = StartCoroutine(SpawnLoop());
    }

    IEnumerator LeashLoop()
    {
        foreach (NetworkObject zombie in spawnedZombieList)
        {
            bool isZombieInsideRange = true;
            foreach (Rigidbody2D rigid2D in playerRigidList)
            {
                Bounds leashRange = new(rigid2D.position + stats.LeashAreaCenter, stats.LeashAreaExtends);
                if (leashRange.Contains(zombie.transform.position))
                    continue;

                isZombieInsideRange = false;
                break;
            }

            if (isZombieInsideRange)
                continue;

            TeleportZombieToPlayerFront(zombie);
        }
        yield return new WaitForSeconds(stats.GetSecondsPerLeash(currentPoolLevel));
        leashLoop = StartCoroutine(LeashLoop());
    }

    private void TeleportZombieToPlayerFront(NetworkObject zombie)
    {
        Rigidbody2D closestRigid = null;
        float closestRigidDist = float.MaxValue;
        foreach (Rigidbody2D rigid2D in playerRigidList)
        {
            float distToZombie = Vector2.Distance(rigid2D.position, zombie.transform.position);
            if (closestRigidDist > distToZombie)
            {
                closestRigidDist = distToZombie;
                closestRigid = rigid2D;
            }
        }

        if (closestRigid == null)
        {
            Debug.LogError("Failed to find closest rigidBody2D");
            return;
        }

        Vector2 spawnPos = GetRandomFrontSpawnPos(closestRigid, zombie.GetComponent<CircleCollider2D>());
        zombie.transform.position = spawnPos;
    }

    public void SpawnZombie(GameObject prefab, Vector2 spawnPos, Quaternion spawnRotation)
    {
        NetworkObject networkObject = NetworkObjectPool.Singleton.GetNetworkObject(prefab, spawnPos, spawnRotation);

        networkObject.Spawn();

        spawnedZombieList.Add(networkObject);
    }


    #region PointStruct

    class Offsets
    {
        public List<Vector2> AllOffsets { get; private set; }
        public List<Vector2> TopOffsets { get; private set; }
        public List<Vector2> BottomOffsets { get; private set; }
        public List<Vector2> LeftOffsets { get; private set; }
        public List<Vector2> RightOffsets { get; private set; }

        public Offsets(Bounds bounds, float distanceBetweenPoints)
        {
            Vector2 topLeft = new(bounds.min.x, bounds.max.y);
            Vector2 topRight = new(bounds.max.x, bounds.max.y);
            Vector2 bottomLeft = new(bounds.min.x, bounds.min.y);
            Vector2 bottomRight = new(bounds.max.x, bounds.min.y);

            AllOffsets = new();

            LeftOffsets = GetPointsBetween(topLeft, bottomLeft, distanceBetweenPoints);
            RightOffsets = GetPointsBetween(topRight, bottomRight, distanceBetweenPoints);

            TopOffsets = GetPointsBetween(topLeft, topRight, distanceBetweenPoints);
            BottomOffsets = GetPointsBetween(bottomLeft, bottomRight, distanceBetweenPoints);
        }

        List<Vector2> GetPointsBetween(Vector2 a, Vector2 b, float distBetweenPoints)
        {
            float distance = Vector2.Distance(a, b);
            int numOfPoints = (int)(distance / distBetweenPoints);

            List<Vector2> points = new();

            for (int i = 0; i < numOfPoints; i++)
            {
                Vector2 pos = Vector2.Lerp(a, b, (float)i / numOfPoints);
                points.Add(pos);
                AllOffsets.Add(pos);
            }

            return points;
        }
    }

    #endregion
}

public enum ZombieID
{
    Bloater,
    Spitter,
    Melee
}