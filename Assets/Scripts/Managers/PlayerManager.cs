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
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerLeft;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnPlayerLeft;
    }

    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!IsServer)
            return;

        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayer(clientID);
        }
    }

    private void OnPlayerJoined(ulong clientID)
    {
        SpawnPlayer(clientID);
    }

    void SpawnPlayer(ulong clientID)
    {
        NetworkObject clientPlayerObject = NetworkObjectPool.Singleton.GetNetworkObject(playerPrefab, carTransform.position, Quaternion.identity);

        Player player = clientPlayerObject.GetComponent<Player>();
        player.PlayerID = clientID;
        PlayerData nullablePlayerData = clientDataMap.TryGetValue(clientID, out PlayerData data) ? data : null;
        player.LoadData(nullablePlayerData);

        playerObjectMap.Add(clientID, clientPlayerObject);

        clientPlayerObject.Spawn();
    }

    private void OnPlayerLeft(ulong obj)
    {
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
}