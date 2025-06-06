using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreBase;
using Data;
using Service;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    public bool isLoadedData = false;
    public UserData userData;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        FileManager.Init(new UnityPathProvider());
        FarmEntityConfigLoader.Load();
        WorkerConfigLoader.Load();
        EquipmentConfigLoader.Load();
        LandConfigLoader.Load();
        Instance = this;
        LoadPlayerData();

    }
    private void OnApplicationQuit()
    {
        SavePlayerData();
    }
    public void LoadPlayerData()
    {
        if (FileManager.Exists("data"))
        {
            userData = FileManager.Load<UserData>("data");
        }
        else
        {
            userData = new UserData();
            userData.InitData();
            SavePlayerData();
        }

        isLoadedData = true;
        InvokeRepeating(nameof(AutoSave), 40f, 40f);
    }
    public void ResetData()
    {
        FileManager.Delete("data");
        userData = new UserData();
        userData.InitData();
        SavePlayerData();
    }
    public void SavePlayerData()
    {
        userData.SaveExitTime(new SystemTimeProvider());
        FileManager.Save(userData, "data");
    }
    private void AutoSave()
    {
        SavePlayerData();

    }
}
