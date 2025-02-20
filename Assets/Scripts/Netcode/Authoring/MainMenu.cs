using System;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    const string world = "WorldScene";
    const string ip = "127.0.0.1";
    const ushort port = 6969;

    [SerializeField] Button startServerButton;
    [SerializeField] Button joinServerButton;

    void Awake()
    {
        startServerButton.onClick.AddListener(StartServer);
        joinServerButton.onClick.AddListener(JoinServer);
    }

    private void StartServer()
    {
        World serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
        World clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");

        foreach (World world in World.All)
        {
            if (world.Flags == WorldFlags.Game)
            {
                world.Dispose();
                break;
            }
        }

        if (World.DefaultGameObjectInjectionWorld == null)
            World.DefaultGameObjectInjectionWorld = serverWorld;

        SceneManager.LoadScene(world, LoadSceneMode.Single);

        RefRW<NetworkStreamDriver> networkStreamDriver =
            serverWorld
                .EntityManager
                    .CreateEntityQuery(typeof(NetworkStreamDriver))
                    .GetSingletonRW<NetworkStreamDriver>();
        networkStreamDriver.ValueRW.Listen(NetworkEndpoint.AnyIpv4.WithPort(port));

        NetworkEndpoint connectNetworkEndpoint = NetworkEndpoint.LoopbackIpv4.WithPort(port);
        networkStreamDriver =
            clientWorld
                .EntityManager
                    .CreateEntityQuery(typeof(NetworkStreamDriver))
                    .GetSingletonRW<NetworkStreamDriver>();
        networkStreamDriver.ValueRW.Connect(clientWorld.EntityManager, connectNetworkEndpoint);

    }

    private void JoinServer()
    {
        World clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");

        foreach (World world in World.All)
        {
            if (world.Flags == WorldFlags.Game)
            {
                world.Dispose();
                break;
            }
        }

        if (World.DefaultGameObjectInjectionWorld == null)
            World.DefaultGameObjectInjectionWorld = clientWorld;

        SceneManager.LoadScene(world, LoadSceneMode.Single);

        NetworkEndpoint connectNetworkEndpoint = NetworkEndpoint.Parse(ip, port);
        RefRW<NetworkStreamDriver> networkStreamDriver =
            clientWorld
                .EntityManager
                    .CreateEntityQuery(typeof(NetworkStreamDriver))
                    .GetSingletonRW<NetworkStreamDriver>();
        networkStreamDriver.ValueRW.Connect(clientWorld.EntityManager, connectNetworkEndpoint);
    }
}
