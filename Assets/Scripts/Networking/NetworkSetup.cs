using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkSetup : MonoBehaviour
{
    [SerializeField] GameObject menuUI;

    [SerializeField] Button host;
    [SerializeField] Button client;
    [SerializeField] Button server;
    void Awake() {
        host.onClick.AddListener(delegate{StartGame(HostType.Host);});
        client.onClick.AddListener(delegate{StartGame(HostType.Client);});
        server.onClick.AddListener(delegate{StartGame(HostType.Server);});
    }

    void StartGame(HostType type) {
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer) return;
        menuUI.SetActive(false);
        switch (type) {
            case HostType.Host:
                NetworkManager.Singleton.StartHost();
                break;
            case HostType.Client:
                NetworkManager.Singleton.StartClient();
                break;
            case HostType.Server:
                NetworkManager.Singleton.StartServer();
                break;
        }
    }
}
