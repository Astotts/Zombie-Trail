using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class OldZombieSpawner : NetworkBehaviour
{
    [SerializeField] GameObject zombiePrefab;
    [SerializeField] float timePerZombie = 1f;
    [SerializeField] SpawnPointGenerator spawnPointGenerator;
    float time;

    float cameraVerticalExtend;
    float cameraHorizontalExtend;

    public override void OnNetworkSpawn()
    {
        if (!IsHost)
        {
            enabled = false;
            return;
        }
        base.OnNetworkSpawn();
        cameraVerticalExtend = Camera.main.orthographicSize;
        cameraHorizontalExtend = cameraVerticalExtend * Screen.width / Screen.height;
    }
    void OnValidate()
    {
        if (timePerZombie < 1f)
            timePerZombie = 1f;
    }

    void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0.0f)
        {
            ResetTimer();
            SpawnZombieAtRandomLocation();
        }
    }
    void ResetTimer()
    {
        time = timePerZombie;
    }

    private void SpawnZombieAtRandomLocation()
    {
        Vector3 randomLocation = GetRandomLocation();
        SpawnZombieAt(randomLocation);
    }

    private Vector3 GetRandomLocation()
    {
        List<Vector3> spawnPoints = spawnPointGenerator.GetAllSpawnPoints();
        List<Vector3> availableSpawnPoints = new();
        foreach (Vector3 spawnPoint in spawnPoints)
        {
            if (IsSpawnPointInScreen(spawnPoint))
                continue;

            availableSpawnPoints.Add(spawnPoint);
        }
        System.Random random = new();
        if (availableSpawnPoints == null)
            return spawnPoints[random.Next(spawnPoints.Count)];
        else
            return availableSpawnPoints[random.Next(availableSpawnPoints.Count)];
    }

    bool IsSpawnPointInScreen(Vector3 spawnPoint)
    {
        GameObject[] onlinePlayers = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < onlinePlayers.Length; i++)
        {
            GameObject player = onlinePlayers[i];

            float minX = player.transform.position.x - cameraHorizontalExtend;
            float maxX = player.transform.position.x + cameraHorizontalExtend;

            float minY = player.transform.position.y - cameraVerticalExtend;
            float maxY = player.transform.position.y + cameraVerticalExtend;

            if (IsSpawnPointBetween(spawnPoint, minX, maxX, minY, maxY))
                return true;
        }
        return false;
    }

    bool IsSpawnPointBetween(Vector3 spawnPoint, float minX, float maxX, float minY, float maxY)
    {
        return minX < spawnPoint.x && spawnPoint.x < maxX && minY < spawnPoint.y && spawnPoint.y < maxY;
    }

    public void SpawnZombieAt(Vector3 spawnLocation)
    {
        GameObject spawnedZombie = Instantiate(zombiePrefab, transform);
        spawnedZombie.transform.position = spawnLocation;

        // Spawn on clients
        NetworkObject spawnedZombieNetworkObject = spawnedZombie.GetComponent<NetworkObject>();
        spawnedZombieNetworkObject.Spawn(true);
    }
}
