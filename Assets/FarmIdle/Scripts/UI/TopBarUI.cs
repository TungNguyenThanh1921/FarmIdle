using System;
using System.Collections;
using System.Collections.Generic;
using CoreBase;
using Observer;
using Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopBarUI : MonoBehaviour
{
    public TextMeshProUGUI goldText, workerText;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameData.Instance != null);
        InitUserMoney();
        ObserverManager.Instance.Attach(EventKeys.UI.UPDATE_MONEY, (Action)InitUserMoney);
    }

    private void OnDestroy()
    {
        ObserverManager.Instance.Detach(EventKeys.UI.UPDATE_MONEY, (Action)InitUserMoney);
    }
    public void InitUserMoney()
    {
        var userData = GameData.Instance.userData;
        goldText.text = $"Gold: {userData.Gold}";
        workerText.text = $"Worker: {GetIdleWorker()}/{userData.Workers.Count}";
    }
    private int GetIdleWorker()
    {
        var workerService = new WorkerService(
            GameData.Instance.userData,
            new InventoryService(GameData.Instance.userData),
            new FarmService(GameData.Instance.userData, new InventoryService(GameData.Instance.userData), new SystemTimeProvider()),
            new SystemTimeProvider()
        );
        return workerService.IdleWorkerCount();
    }

    private void OnSellClick()
    {
        // mở popup Sell
        Debug.Log("🛒 Mở popup bán sản phẩm");
    }
}
