using Unity.Netcode;
using UnityEngine;

public class CarPassenger : NetworkBehaviour
{
    [SerializeField] Rigidbody2D rigid2D;
    [SerializeField] CarStats _carStats;
    public CarStats Stats => _carStats;

    private NetworkVariable<int> currentPassengers = new();
    private bool IsFull => currentPassengers.Value >= 0;

    public bool AddPassenger(int amount)
    {
        if (!IsHost)
            return false;

        currentPassengers.Value += amount;
        return IsFull;
    }
}