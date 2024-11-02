using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CarManager : MonoBehaviour
{
    public static CarManager Instance;
    [SerializeField] List<CarStats> carStats;
    [SerializeField] GameObject carPrefab;

    [SerializeField] CarUpgrades healthUpgradeUI;
    [SerializeField] CarUpgrades damageUpgradeUI;
    [SerializeField] CarUpgrades speedUpgradeUI;
    [SerializeField] CarUpgrades capacityUpgradeUI;

    Car currentSelectedCar;

    readonly List<Car> cars = new();
    SpriteRenderer prefabSpriteRender;
    CarMovement prefabMovement;
    CarHealth prefabHealth;
    CarPassenger prefabPassenger;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        Instance = this;
        prefabSpriteRender = carPrefab.GetComponent<SpriteRenderer>();
        prefabMovement = carPrefab.GetComponent<CarMovement>();
        prefabHealth = carPrefab.GetComponent<CarHealth>();
        prefabPassenger = carPrefab.GetComponent<CarPassenger>();

        // Initialize all car object
        foreach (CarStats stat in carStats)
        {
            Car newCar = new(stat);
            cars.Add(newCar);
        }

        currentSelectedCar = cars[0];
    }

    public void SpawnCurrentSelectedCar()
    {
        SpawnCarAt(currentSelectedCar, Vector2.zero);
    }

    public void SpawnCarAt(Car car, Vector2 spawnLocation)
    {
        // Setup the stats before spawning
        prefabSpriteRender.sprite = car.GetSprite();
        prefabHealth.SetMaxHealth(car.GetStat(ECarStatName.HEALTH));
        prefabMovement.SetSpeed(car.GetStat(ECarStatName.SPEED));
        prefabPassenger.SetCapacity(car.GetStat(ECarStatName.CAPACITY));

        // Spawn the car
        GameObject spawned = Instantiate(carPrefab);
        spawned.GetComponent<NetworkObject>().Spawn();
        spawned.transform.position = spawnLocation;
    }

    public void UpgradeHealth()
    {
        currentSelectedCar.Upgrade(ECarStatName.HEALTH);
        healthUpgradeUI.SetLevel(currentSelectedCar.GetStat(ECarStatName.HEALTH));
    }

    public void UpgradeDamage()
    {
        currentSelectedCar.Upgrade(ECarStatName.DAMAGE);
        damageUpgradeUI.SetLevel(currentSelectedCar.GetStat(ECarStatName.DAMAGE));
    }

    public void UpgradeSpeed()
    {
        currentSelectedCar.Upgrade(ECarStatName.SPEED);
        speedUpgradeUI.SetLevel(currentSelectedCar.GetStat(ECarStatName.SPEED));
    }

    public void UpgradeCapacity()
    {
        currentSelectedCar.Upgrade(ECarStatName.CAPACITY);
        speedUpgradeUI.SetLevel(currentSelectedCar.GetStat(ECarStatName.CAPACITY));
    }
}

[Serializable]
public class CarUpgrades
{
    static readonly int NUMBER_OF_NODE = 8;
    [SerializeField] Button button;
    [SerializeField] RectTransform nodeTransform;
    [SerializeField] RectMask2D nodeMask;

    public void Lock()
    {
        button.interactable = false;
    }

    public void SetLevel(int level)
    {
        float width = nodeTransform.rect.width;
        nodeMask.padding = new Vector4(0, width / NUMBER_OF_NODE * level, 0, 0);
        if (level >= NUMBER_OF_NODE)
        {
            Lock();
        }
    }
}