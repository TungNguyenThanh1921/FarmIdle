using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using CoreGamePlay;
using Data;
using Service;

public class ToolPopup : PopupTemplate
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private GameObject itemPrefab;

    private UserData userData;
    private InventoryService inventory;
    private EquipmentService equipmentService;

    public override void UpdateInformation(object _data)
    {
        userData = GameData.Instance.userData;
        inventory = new InventoryService(userData);
        equipmentService = new EquipmentService(userData, inventory);
        Refresh();
    }


    public void Refresh()
    {
        foreach (Transform t in contentRoot)
            Destroy(t.gameObject);

        foreach (var config in EquipmentConfigLoader.All.Values)
        {
            var go = Instantiate(itemPrefab, contentRoot);
            var ui = go.GetComponent<EquipmentItemUI>();
            var currentLevel = (userData.Equipment?.Config.Id == config.Id) ? userData.Equipment.Level : 0;

            ui.Set(config, currentLevel, () =>
            {
                if (equipmentService.TryUpgrade(config.Id))
                {
                    Refresh();
                }
            });
        }
    }

}