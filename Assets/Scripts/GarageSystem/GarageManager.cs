using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GarageManager : MonoBehaviour
{
    public static GarageManager Instance { get; private set; }

    [SerializeField] Vector2 carStartLocation;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Vector2 spawnOffset;

    void Awake()
    {
        if (Instance != null)
            return;
        Instance = this;
        DontDestroyOnLoad(gameObject);
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnWorldSceneLoaded;
    }

    void OnDestroy()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnWorldSceneLoaded;
    }

    public void Explore()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("World", LoadSceneMode.Single);
    }

    public void OnWorldSceneLoaded(ulong clientID, string sceneName, LoadSceneMode mode)
    {
        if (sceneName != "World" || !NetworkManager.Singleton.IsHost)
            return;

        CarManager.Instance.SpawnCurrentSelectedCarAt(carStartLocation);
        int offset = 0;
        foreach (ulong userId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            GameObject player = Instantiate(playerPrefab, spawnOffset + new Vector2(offset, 0), Quaternion.identity);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(userId, true);
            offset++;
        }
    }
}
