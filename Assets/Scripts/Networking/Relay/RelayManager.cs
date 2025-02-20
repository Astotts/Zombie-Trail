using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Xml;
using Unity.Netcode;
//using Unity.Netcode.Editor;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
//using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance { get; private set; }
    public static string JoinCode;

    public const int MAX_PLAYER = 4;

    bool isConnecting;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    async void Start()
    {
        System.Random random = new();
        InitializationOptions options = new();
        options.SetProfile(random.Next(10000).ToString());

        await UnityServices.InitializeAsync(options);

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        if (isConnecting)
            return;
        isConnecting = true;

        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MAX_PLAYER - 1); // Max player = host + others

            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log(JoinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnWorldSceneLoaded;
            NetworkManager.Singleton.SceneManager.OnUnloadComplete += OnWorldSceneUnloaded;

            NetworkManager.Singleton.SceneManager.LoadScene("World", LoadSceneMode.Single);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }

        isConnecting = false;
    }

    private void OnWorldSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (sceneName != "World")
            return;

        WorldDataManager.Instance.LoadWorld("World");
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnWorldSceneLoaded;
    }

    private void OnWorldSceneUnloaded(ulong clientId, string sceneName)
    {
        if (sceneName != "World" || clientId != NetworkManager.Singleton.ConnectedClientsIds[0])
            return;

        WorldDataManager.Instance.SaveWorld("World");
    }

    public async void JoinRelay(string code)
    {
        if (isConnecting)
            return;
        isConnecting = true;

        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(code);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }

        isConnecting = false;
    }
}
