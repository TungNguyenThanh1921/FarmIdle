using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
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

        Instance = this;
        LoadPlayerData();

    }

    public void LoadPlayerData()
    {
        if (FileManager.Exists("data"))
        {
            userData.ClearData();
            userData = FileManager.Load<UserData>("data");
        }
        else
        {
            userData.InitData();
            SavePlayerData();
        }
        isLoadedData = true;
        InvokeRepeating(nameof(AutoSave), 40f, 40f);
    }
    public void SavePlayerData()
    {
        FileManager.Save(userData, "data");
    }
    private void AutoSave()
    {
        SavePlayerData();

    }
}
