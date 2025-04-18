using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using CoreGamePlay;
using Data;
using Service;
using Observer;

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

        var equipmentList = userData.Equipments;
        foreach (var equip in equipmentList)
        {
            var config = equip.Config;
            var go = Instantiate(itemPrefab, contentRoot);
            var ui = go.GetComponent<EquipmentItemUI>();
            ui.gameObject.SetActive(true);

            int currentLevel = equip.Level;
            int upgradeCost = EquipmentService.CalculateUpgradeCost(config, currentLevel + 1);
            double percentBoost = equip.GetPercentBoost();

            ui.Set(config, percentBoost, currentLevel, upgradeCost, () =>
            {
                if (equipmentService.TryUpgrade(config.Id))
                {
                    Refresh();
                    ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_MONEY);
                }
            });
        }

        itemPrefab.SetActive(false);
    }

}