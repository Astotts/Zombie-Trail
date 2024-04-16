using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] GameObject lobbyUI;
    public static LobbyManager instance;
    public HostType type;

    [SerializeField] Button host;
    [SerializeField] Button client;
    [SerializeField] Button server;

    void Awake() {
        instance = this;
        DontDestroyOnLoad(gameObject);
        host.onClick.AddListener(delegate{StartGame(HostType.Host);});
        client.onClick.AddListener(delegate{StartGame(HostType.Client);});
        server.onClick.AddListener(delegate{StartGame(HostType.Server);});
    }

    public void StartGame(HostType type) {
        StartCoroutine(SetType(SceneManager.LoadSceneAsync("SampleScene"), type));
    }

    IEnumerator SetType(AsyncOperation asyncLoad, HostType type) {
        while (!asyncLoad.isDone) {
            yield return null;
        }
        this.type = type;
    }
}

public enum HostType {
    Unset,
    Host,
    Client,
    Server
}
