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
    public event EventHandler<NetworkObject> OnPlayerSpawned;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform carTransform;
    readonly Dictionary<ulong, Player> playerMap = new();
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
        Debug.Log("Player " + clientId + " left, Saving Data");
        Player player = playerMap[clientId];
        PlayerData data = clientDataMap[clientId];
        player.SaveData(ref data);
        playerMap.Remove(clientId);
    }

    private void OnPlayerJoined(ulong clientID)
    {
        SpawnPlayer(clientID);
    }

    void SpawnPlayer(ulong clientID)
    {
        GameObject spawnedPlayerObject = Instantiate(playerPrefab, carTransform.position, Quaternion.identity, transform);
        NetworkObject playerNetworkObject = spawnedPlayerObject.GetComponent<NetworkObject>();
        Player player = spawnedPlayerObject.GetComponent<Player>();

        playerMap[clientID] = player;

        playerNetworkObject.SpawnAsPlayerObject(clientID);
        playerNetworkObject.TrySetParent(transform);

        player.PlayerID = clientID;

        if (clientDataMap.TryGetValue(clientID, out PlayerData data))
            player.LoadData(data);
        else
            clientDataMap.Add(clientID, new PlayerData());

        OnPlayerSpawned?.Invoke(this, playerNetworkObject);
    }

    public void LoadData(WorldData worldData)
    {
        if (worldData.ClientDataMap == null)
            return;
        clientDataMap = worldData.ClientDataMap;
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (clientDataMap.TryGetValue(clientID, out PlayerData data))
                playerMap[clientID].LoadData(data);
            else
                clientDataMap.Add(clientID, new PlayerData());
        }
    }

    public void SaveData(ref WorldData worldData)
    {
        worldData.ClientDataMap = clientDataMap;
    }
}