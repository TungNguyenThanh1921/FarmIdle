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

            // Giả lập offline 1 giờ
            var fakeNow = DateTime.Parse("2025-04-19T23:32:24");
            var timeProvider = new FakeTimeProvider(fakeNow);

            var inventoryService = new InventoryService(userData);
            var farmService = new FarmService(userData, inventoryService, timeProvider);
            var workerService = new WorkerService(userData, inventoryService, farmService, timeProvider);

            // workerService sẽ tự động gọi ProcessOfflineWork() trong constructor
            // Xem kết quả sau khi xử lý
            Debug.Log("Test: Tomato: " + userData.Inventory["Tomato"]);
            Debug.Log("Test: Milk: " + userData.Inventory["Milk"]);
            Debug.Log("Test: Blueberry: " + userData.Inventory["Blueberry"]);
            Debug.Log("Test: Strawberry: " + userData.Inventory["Strawberry"]);
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
