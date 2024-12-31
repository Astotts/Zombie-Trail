using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : NetworkBehaviour, IPersistentData
{
    public static PlayerManager Instance { get; private set; }
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform carTransform;
    Dictionary<ulong, NetworkObject> playerObjectMap = new();
    Dictionary<ulong, PlayerData> clientDataMap = new();

    void Awake()
    {
        if (Instance != null)
            Debug.LogError("There are more than one Player Manager!");
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        base.OnNetworkSpawn();
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayer(clientID);
        }
        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPLayerLeft;
        NetworkManager.Singleton.OnServerStopped += OnServerStop;
    }

    private void OnServerStop(bool obj)
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnPLayerLeft;
        NetworkManager.Singleton.OnServerStopped -= OnServerStop;
    }

    private void OnPLayerLeft(ulong clientId)
    {
        playerObjectMap.Remove(clientId);
    }

    private void OnPlayerJoined(ulong clientID)
    {
        SpawnPlayer(clientID);
    }

    void SpawnPlayer(ulong clientID)
    {
        GameObject spawnedPlayerObject = Instantiate(playerPrefab, carTransform.position, Quaternion.identity, transform);

        Player player = spawnedPlayerObject.GetComponent<Player>();
        PlayerData nullablePlayerData = clientDataMap.TryGetValue(clientID, out PlayerData data) ? data : null;
        player.LoadData(nullablePlayerData);

        NetworkObject playerNetworkObject = spawnedPlayerObject.GetComponent<NetworkObject>();

        playerObjectMap.Add(clientID, playerNetworkObject);

        playerNetworkObject.SpawnAsPlayerObject(clientID);
        player.PlayerID = clientID;
    }

    public void LoadData(WorldData worldData)
    {
        if (worldData.clientDataMap == null)
            return;
        clientDataMap = worldData.clientDataMap;
    }

    public void SaveData(ref WorldData worldData)
    {
        worldData.clientDataMap = clientDataMap;
    }

    public NetworkObject GetPlayerObject(ulong playerId)
    {
        return playerObjectMap[playerId];
    }
}