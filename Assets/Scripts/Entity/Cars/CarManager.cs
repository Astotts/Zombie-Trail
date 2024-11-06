using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CarManager : NetworkBehaviour
{
    public static CarManager Instance { get; private set; }
    [SerializeField] List<CarStats> carStats;
    [SerializeField] GameObject carPrefab;

    [SerializeField] Image carPreviewUI;

    [SerializeField] CarUpgrades healthUpgradeUI;
    [SerializeField] CarUpgrades damageUpgradeUI;
    [SerializeField] CarUpgrades speedUpgradeUI;
    [SerializeField] CarUpgrades capacityUpgradeUI;

    int currentSelectedCarIndex = 0;
    public Car CurrentSelectedCar { get; private set; }

    readonly List<Car> cars = new();
    SpriteRenderer prefabSpriteRender;
    CarMovement prefabMovement;
    CarHealth prefabHealth;
    CarPassenger prefabPassenger;

    void Awake()
    {
        Instance = this;

        // Initialize all car object
        foreach (CarStats stat in carStats)
        {
            Car newCar = new(stat);
            cars.Add(newCar);
        }
        CurrentSelectedCar = cars[currentSelectedCarIndex];
        CurrentSelectedCar.IsUnlocked = true;
    }

    public void SpawnCurrentSelectedCarAt(Vector2 location)
    {
        SpawnCarAt(CurrentSelectedCar, location);
    }

    public void SpawnCarAt(Car car, Vector2 spawnLocation)
    {

        // Spawn the car
        GameObject spawned = Instantiate(carPrefab);
        spawned.transform.position = spawnLocation;

        // Setup the stats
        prefabSpriteRender = spawned.GetComponent<SpriteRenderer>();
        prefabMovement = spawned.GetComponent<CarMovement>();
        prefabHealth = spawned.GetComponent<CarHealth>();
        prefabPassenger = spawned.GetComponent<CarPassenger>();

        prefabSpriteRender.sprite = car.GetSprite();
        prefabHealth.SetMaxHealth(car.GetStat(ECarStatName.HEALTH));
        prefabMovement.SetSpeed(car.GetStat(ECarStatName.SPEED));
        prefabPassenger.SetCapacity(car.GetStat(ECarStatName.CAPACITY));

        spawned.GetComponent<NetworkObject>().Spawn();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UpgradeRpc(ECarStatName statName)
    {
        CurrentSelectedCar.Upgrade(statName);
        int currentUpgradeLevel = CurrentSelectedCar.GetUpgradeLevel(statName);
        switch (statName)
        {
            case ECarStatName.HEALTH:
                healthUpgradeUI.SetLevel(currentUpgradeLevel);
                break;
            case ECarStatName.DAMAGE:
                damageUpgradeUI.SetLevel(currentUpgradeLevel);
                break;
            case ECarStatName.SPEED:
                speedUpgradeUI.SetLevel(currentUpgradeLevel);
                break;
            case ECarStatName.CAPACITY:
                capacityUpgradeUI.SetLevel(currentUpgradeLevel);
                break;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SelectNextCarRpc()
    {
        currentSelectedCarIndex = Modulo(currentSelectedCarIndex + 1, cars.Count);
        CurrentSelectedCar = cars[currentSelectedCarIndex];
        carPreviewUI.sprite = CurrentSelectedCar.GetSprite();
        healthUpgradeUI.SetLevel(CurrentSelectedCar.GetUpgradeLevel(ECarStatName.HEALTH));
        damageUpgradeUI.SetLevel(CurrentSelectedCar.GetUpgradeLevel(ECarStatName.DAMAGE));
        speedUpgradeUI.SetLevel(CurrentSelectedCar.GetUpgradeLevel(ECarStatName.SPEED));
        capacityUpgradeUI.SetLevel(CurrentSelectedCar.GetUpgradeLevel(ECarStatName.CAPACITY));
        if (CurrentSelectedCar.IsUnlocked)
            carPreviewUI.color = Color.white;
        else
            carPreviewUI.color = Color.black;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SelectPreviousCarRpc()
    {
        currentSelectedCarIndex = Modulo(currentSelectedCarIndex - 1, cars.Count);

        CurrentSelectedCar = cars[currentSelectedCarIndex];
        carPreviewUI.sprite = CurrentSelectedCar.GetSprite();
        healthUpgradeUI.SetLevel(CurrentSelectedCar.GetUpgradeLevel(ECarStatName.HEALTH));
        damageUpgradeUI.SetLevel(CurrentSelectedCar.GetUpgradeLevel(ECarStatName.DAMAGE));
        speedUpgradeUI.SetLevel(CurrentSelectedCar.GetUpgradeLevel(ECarStatName.SPEED));
        capacityUpgradeUI.SetLevel(CurrentSelectedCar.GetUpgradeLevel(ECarStatName.CAPACITY));
        if (CurrentSelectedCar.IsUnlocked)
            carPreviewUI.color = Color.white;
        else
            carPreviewUI.color = Color.black;
    }

    int Modulo(int a, int b)
    {
        int mod = a % b;
        return a < 0 ? mod + b : mod;
    }
}

[Serializable]
public class CarUpgrades
{
    static readonly int NUMBER_OF_NODE = 8;
    static readonly int MENU_SCALE = 3;
    [SerializeField] Button button;
    [SerializeField] RectTransform nodeTransform;
    [SerializeField] RectMask2D nodeMask;

    public void Lock()
    {
        button.interactable = false;
    }

    public void SetLevel(int level)
    {
        float width = nodeTransform.rect.width * MENU_SCALE;
        nodeMask.padding = new Vector4(0, 0, width / NUMBER_OF_NODE * (NUMBER_OF_NODE - level - 1), 0);
        if (level >= NUMBER_OF_NODE)
        {
            Lock();
        }
    }
}