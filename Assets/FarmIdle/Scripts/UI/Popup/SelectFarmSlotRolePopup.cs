using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectFarmSlotRolePopup : PopupTemplate
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Button applyButton;

    private FarmSlotUIItem data;
    private List<string> availableIds = new();

    public override void UpdateInformation(object _data)
    {
        data = _data as FarmSlotUIItem;
        if (data == null)
        {
            Debug.LogError("Dữ liệu truyền vào popup không hợp lệ");
            return;
        }

        dropdown.ClearOptions();
        availableIds.Clear();

        var options = new List<TMP_Dropdown.OptionData>();
        foreach (var entry in FarmEntityConfigLoader.All)
        {
            var cfg = entry.Value;
            if (cfg.Type != "Crop" && cfg.Type != "Animal") continue;

            availableIds.Add(cfg.Id);
            options.Add(new TMP_Dropdown.OptionData(cfg.DisplayName));
        }

        dropdown.AddOptions(options);
        applyButton.interactable = availableIds.Count > 0;

        applyButton.onClick.RemoveAllListeners();
        applyButton.onClick.AddListener(OnClickApply);
    }

    private void OnClickApply()
    {
        if (availableIds.Count == 0) return;

        string selectedId = availableIds[dropdown.value];
        bool result = data.farmService.TryAssignRoleToSlot(data.slotIndex, selectedId);
        if (result)
        {
            Debug.Log($"Gán loại {selectedId} cho slot {data.slotIndex}");
            data.RefreshUI(); // nếu có
            PopupManager.Instance.ClosePopup();
        }
        else
        {
            Debug.LogWarning($"Gán loại thất bại: {selectedId}");
        }
    }
}