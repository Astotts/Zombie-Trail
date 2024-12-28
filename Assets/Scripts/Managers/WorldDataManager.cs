using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class WorldDataManager : MonoBehaviour
{
    public static WorldDataManager Instance { get; set; }
    [SerializeField] List<GameObject> objectsToSaveList = new();
    [SerializeField] string directoryToSave = "/Worlds";

    readonly List<IPersistentData> persistentDatas = new();
    WorldData worldData;

    string worldName;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There are more than one PersistentDataManager!");
            return;
        }
        Instance = this;
        GetAllPersistenDataInterfaces();
    }

    void OnDestroy()
    {
        Instance = null;
    }

    public void SaveWorld()
    {
        SaveWorld(worldName);
    }

    public void SaveWorld(string worldName)
    {
        this.worldName = worldName;
        foreach (IPersistentData persistentData in persistentDatas)
        {
            worldData ??= new WorldData();
            persistentData.SaveData(ref worldData);
        }

        JsonUtil<WorldData>.SaveToFile(worldData, Application.persistentDataPath + directoryToSave, worldName + ".json");
    }

    public void LoadWorld(string worldName)
    {
        this.worldName = worldName;
        worldData = JsonUtil<WorldData>.LoadFromFile(Application.persistentDataPath + directoryToSave, worldName + ".json");

        foreach (IPersistentData persistentData in persistentDatas)
        {
            persistentData.LoadData(worldData);
        }
    }

    void OnApplicationQuit()
    {
        SaveWorld();
    }

    void GetAllPersistenDataInterfaces()
    {
        foreach (GameObject persistGO in objectsToSaveList)
        {
            IPersistentData[] datas = persistGO.GetComponents<IPersistentData>();
            foreach (IPersistentData data in datas)
            {
                persistentDatas.Add(data);
            }
        }
    }
}

public interface IPersistentData
{
    void LoadData(WorldData worldData);
    void SaveData(ref WorldData worldData);
}

public class WorldData
{
    public string WorldName { get; set; }
    public long TimeSpent { get; set; }
    // The whole purpose is to serialize a dictionary
    // Json.net won't accept a complex data for dictionary key
    // The reason? beat me
    public List<KeyValuePair<int[], List<StructureData>>> loadedStructuresData;
}

public class JsonUtil<T>
{
    public static void SaveToFile(T data, string directory, string fileName)
    {
        string fullPath = Path.Combine(directory, fileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string jsonString = JsonConvert.SerializeObject(data);

            using FileStream stream = new(fullPath, FileMode.Create);
            using StreamWriter writer = new(stream);
            writer.Write(jsonString);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save data to file: " + fullPath + "\n" + e);
        }
    }

    public static T LoadFromFile(string directory, string fileName)
    {
        string fullPath = Path.Combine(directory, fileName);
        T data = default;
        if (!File.Exists(fullPath))
            return data;

        try
        {
            string jsonString;
            using FileStream stream = new(fullPath, FileMode.Open);
            using StreamReader reader = new(stream);
            jsonString = reader.ReadToEnd();

            data = JsonConvert.DeserializeObject<T>(jsonString);
            Debug.Log("Saved data at: " + fullPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load data from file: " + fullPath + "\n" + e);
        }
        return data;
    }
}