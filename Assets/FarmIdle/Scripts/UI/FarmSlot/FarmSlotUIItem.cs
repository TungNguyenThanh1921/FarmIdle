using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CoreGamePlay;
using Service;
using System;

public class FarmSlotUIItem : MonoBehaviour
{
    [Header("UI Refs")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI slotedText;

    public TextMeshProUGUI timeText;
    public Button plantButton;
    public Button harvestButton;
    public Button buttonPlus;
    public int slotIndex;
    public FarmService farmService;
    private FarmSlot slot;

    public void Init(int index, FarmSlot slot, FarmService service)
    {
        this.slotIndex = index;
        this.slot = slot;
        this.farmService = service;

        RefreshUI();

        plantButton.onClick.RemoveAllListeners();
        plantButton.onClick.AddListener(OnPlantClick);

        harvestButton.onClick.RemoveAllListeners();
        harvestButton.onClick.AddListener(OnHarvestClick);

        buttonPlus.onClick.RemoveAllListeners();
        buttonPlus.onClick.AddListener(AddLandRole);
    }
    public void AddLandRole()
    {
        PopupManager.instance.OpenPopup(PopupIDs.SelectFarmSlotRole, this);
    }

    public void RefreshUI()
    {
        if (slot.LockedType == null)
        {
            nameText.text = $"Slot {slotIndex}: Đất trống";
            statusText.text = $"Chưa có cây trồng";
            timeText.text = "";
            slotedText.text = "";

            buttonPlus.gameObject.SetActive(true);
            plantButton.gameObject.SetActive(false);
            harvestButton.gameObject.SetActive(false);

            return;
        }
        buttonPlus.gameObject.SetActive(false);
        slotedText.text = "Số ô đã trồng: " + slot.TotalPlantedSlot().ToString();
        string product = slot.GetProductName();
        nameText.text = $"Slot {slotIndex}: {product}";

        int totalYielded = slot.GetTotalYielded();
        int maxYield = slot.GetTotalMaxYield();
        statusText.text = $"Cây: {slot.Entities.Count}/5 | Thu hoạch: {totalYielded}/{maxYield}";

        timeText.text = slot.GetRemainingTimeText(DateTime.Now);

        plantButton.gameObject.SetActive(!slot.IsFull);
        harvestButton.gameObject.SetActive(slot.CanHarvestAny(DateTime.Now));
    }

    private void OnPlantClick()
    {
        bool success = farmService.TryPlantAtSlot(slotIndex);
        if (success)
        {
            RefreshUI();
            Debug.Log($"✅ Đã trồng vào slot {slotIndex}");
        }
        else
        {
            Debug.LogWarning($"❌ Không thể trồng ở slot {slotIndex}");
        }
    }

    private void OnHarvestClick()
    {
        int amount = farmService.HarvestSlot(slotIndex);
        if (amount > 0)
        {
            RefreshUI();
            Debug.Log($"🧺 Thu hoạch {amount} sản phẩm từ slot {slotIndex}");
        }
        else
        {
            Debug.Log($"⚠️ Không có gì để thu hoạch ở slot {slotIndex}");
        }
    }
}