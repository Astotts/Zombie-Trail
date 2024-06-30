using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;                     // To keep track of one GameManager
    public WaveManager wm;
    //[SerializeField] float playerHealth;                      // Player's current health
    List<GameObject> zombiesInTheLevel;                         // Keeps track of the zombies in the level. Allows for wave start/end

    bool isWaveActive = false;

    public static event Action<GameState> OnStateChange;
    public static GameState currentState = GameState.WaveStart;

    readonly NetworkVariable<int> networkState = new(3);


    // This allows us to keep the GameManager script when scenes are reloading or changing
    void Awake()
    {
        zombiesInTheLevel = new List<GameObject>();

        wm = FindObjectOfType<WaveManager>();

        // Instance of this object already around?
        if (Instance != null)
        {
            // Not anymore dude...
            Destroy(gameObject);    // Destroy this object, another exists. I cry.
            return;
        }

        // This is the first, congrats, you live
        Instance = this;
        Debug.Log("GameManager awaken");
    }

    public override void OnNetworkSpawn()
    {
        DontDestroyOnLoad(gameObject);      // Don't destroy you, you lucky

        // Not server? Well go ask the server for current state
        if (IsHost || IsServer)
            StateUpdateClientRpc(GameState.WaitEnd);
        else
            SyncGameState();
        Debug.Log("GameManager network spawned");
    }

    public void AddZombieToList(GameObject zombie)
    {
        zombiesInTheLevel.Add(zombie);

        if (!isWaveActive)
        {
            isWaveActive = true; // Wave has started. This is the first zombie in the wave
            StateUpdateClientRpc(GameState.WaveStart);
        }
    }

    public void RemoveZombieFromList(GameObject zombie)
    {
        Debug.Log("Remove Zombie called");
        zombiesInTheLevel.Remove(zombie);

        if (IsWaveOver())
        {
            StateUpdateClientRpc(GameState.WaveEnd);
        }
    }

    public bool IsWaveOver()
    {
        Debug.Log("Number of zombers left: " + zombiesInTheLevel.Count);
        if(zombiesInTheLevel.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void StateUpdateClientRpc(GameState state)
    {
        switch (state)
        {
            case GameState.WaveStart:
                Debug.Log("Wave has started");
                currentState = state;
                OnStateChange?.Invoke(state);
                break;
            case GameState.WaveEnd:
                Debug.Log("Wave Over. Waiting");
                currentState = state;
                OnStateChange?.Invoke(state);
                break;
            case GameState.WaitStart:
                Debug.Log("It's Shopping Time");
                currentState = state;
                OnStateChange?.Invoke(state);
                break;
            case GameState.WaitEnd:
                Debug.Log("Shopping Over. New Wave starting...");
                currentState = state;
                OnStateChange?.Invoke(state);
                break;
        }

        if (IsServer || IsHost)
            networkState.Value = (int) currentState;
    }

    void SyncGameState() {
        Debug.Log("Synchronized gameState late join");
        StateUpdateClientRpc((GameState) networkState.Value);
    }
}

public enum GameState
{
    WaveStart,
    WaveEnd,
    WaitStart,
    WaitEnd
}