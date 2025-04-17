using System;
using System.Collections;
using System.Collections.Generic;
using Observer;
using Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopBarUI : MonoBehaviour
{
    public TextMeshProUGUI goldText, workerText;
    public Button sellButton;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameData.Instance != null);
        sellButton.onClick.AddListener(OnSellClick);
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
        return new WorkerService(GameData.Instance.userData).IdleWorkerCount(System.DateTime.Now);
    }

    private void OnSellClick()
    {
        // mở popup Sell
        Debug.Log("🛒 Mở popup bán sản phẩm");
    }
}
