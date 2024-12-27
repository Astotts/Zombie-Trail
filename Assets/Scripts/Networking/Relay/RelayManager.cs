using System.Collections;
using System.Collections.Generic;
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
    bool IsLocked;

    public const int MAX_PLAYER = 4;

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
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MAX_PLAYER - 1); // Max player = host + others

            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log(JoinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;

            NetworkManager.Singleton.StartHost();

            // NetworkManager.Singleton.SceneManager.LoadScene("Garage", LoadSceneMode.Single);
            NetworkManager.Singleton.SceneManager.LoadScene("World", LoadSceneMode.Single);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.Reason = "The owners is currently away from the base, please wait for them to return.";
    }

    public async void JoinRelay(string code)
    {
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
    }

    public void LockRelay()
    {
        IsLocked = true;
    }

    public void UnLockRelay()
    {
        IsLocked = false;
    }
}
