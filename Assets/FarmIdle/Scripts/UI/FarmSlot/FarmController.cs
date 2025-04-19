using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CoreGamePlay;
using Service;
using Data;
using CoreBase;
using System.Collections.Generic;
using System.Collections;
using Observer;
using System;

public class FarmController : MonoBehaviour
{
    [Header("UI")]
    public Transform contentRoot;
    public GameObject farmSlotItemPrefab;

    private FarmService farmService;
    private InventoryService inventoryService;
    private UserData userData;
    public WorkerService workerService;
    private List<FarmSlotUIItem> slotUIItems = new();
    private ITimeProvider timeProvider;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => GameData.Instance != null);
        InitServices();
        InitSlotUI();
        ObserverManager.Instance.Attach(EventKeys.UI.UPDATE_LAND, (Action)InitSlotUI);
    }
    void OnDestroy()
    {
        ObserverEventManager.detach(EventKeys.UI.UPDATE_LAND, (Action)InitSlotUI);
        StopAllCoroutines();
    }
    private void InitServices()
    {
        // 🧠 Tạo dữ liệu và các service cần thiết
        userData = GameData.Instance.userData; // hoặc tạo mới
        inventoryService = new InventoryService(userData);
        timeProvider = new SystemTimeProvider();
        farmService = new FarmService(userData, inventoryService, new SystemTimeProvider());
        workerService = new WorkerService(userData, inventoryService, farmService, timeProvider);
        workerService.RebindAssignedSlots();
        StartCoroutine(TickCheckWorker());
    }
    IEnumerator TickCheckWorker()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            workerService.Tick();
        }
    }
    private void InitSlotUI()
    {
        // Làm sạch scroll content
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);
        slotUIItems.Clear();

        // Tạo từng item UI
        for (int i = 0; i < userData.Slots.Count; i++)
        {
            var go = Instantiate(farmSlotItemPrefab, contentRoot);
            var uiItem = go.GetComponent<FarmSlotUIItem>();
            uiItem.gameObject.SetActive(true);
            uiItem.Init(i, userData.Slots[i], farmService, timeProvider);
            slotUIItems.Add(uiItem);
        }
        farmSlotItemPrefab.SetActive(false);
    }

}