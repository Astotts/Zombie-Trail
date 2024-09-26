using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PlayerSpawn : NetworkBehaviour
{
    public GameObject spawnPoint;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn(); // Call base method

        if (IsLocalPlayer) // Ensure this is the local player
        {
            StartCoroutine(FindSpawnPointCoroutine());
        }
    }

    private IEnumerator FindSpawnPointCoroutine()
    {
        // Keep searching for the spawn point until it is found
        while (spawnPoint == null)
        {
            spawnPoint = GameObject.FindWithTag("PlayerSpawn");
            
            if (spawnPoint == null)
            {
                Debug.LogWarning("SpawnPoint not found, searching...");
                yield return new WaitForSeconds(1f); // Wait for 1 second before the next search
            }
        }

        // Set the player's position to the spawn point's position once found
        transform.position = spawnPoint.transform.position;
        transform.rotation = spawnPoint.transform.rotation; // Optional: set rotation
    }
}
