using Unity.Netcode;
using UnityEngine;

public class CarPassenger : NetworkBehaviour
{
    [SerializeField] Rigidbody2D rigid2D;
    int capacity;
    private readonly NetworkVariable<int> currentPassengers = new();

    public bool AddPassenger(int amount)
    {
        if (!IsHost)
            return false;

        if (IsFull())
        {
            return false;
        }

        currentPassengers.Value += amount;
        return true;
    }

    private bool IsFull()
    {
        return currentPassengers.Value >= capacity;
    }

    public void SetCapacity(int amount)
    {
        capacity = amount;
    }
}