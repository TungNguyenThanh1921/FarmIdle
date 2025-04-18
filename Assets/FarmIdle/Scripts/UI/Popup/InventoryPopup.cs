using Observer;
using Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPopup : PopupTemplate
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Button sellButton;

    private InventoryService inventory;

    public override void UpdateInformation(object _data)
    {
        inventory = new InventoryService(GameData.Instance.userData);
        RefreshUI();

        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() =>
        {
            inventory.SellAllHarvestedProducts();
            RefreshUI();
            ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_MONEY);
        });
    }

    private void RefreshUI()
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        foreach (var kvp in inventory.GetAllItems())
        {
            var go = Instantiate(itemPrefab, contentRoot);
            go.SetActive(true);
            InventoryItem txt = go.GetComponent<InventoryItem>();
            txt.InitData($"{kvp.Key}: {kvp.Value}");
        }
        itemPrefab.SetActive(false);
    }
}