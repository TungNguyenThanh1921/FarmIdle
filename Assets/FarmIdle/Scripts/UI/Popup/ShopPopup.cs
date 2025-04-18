using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CoreGamePlay;
using Service;
using Data;
using Observer;

public class ShopPopup : PopupTemplate
{
    [SerializeField] private TMP_Dropdown dropdownCategory;
    [SerializeField] private RectTransform contentRoot;
    [SerializeField] private GameObject itemPrefab;

    private ShopService shopService;
    private InventoryService inventoryService;
    private UserData userData;

    private enum Category
    {
        CropAnimal,
        Land,
        Worker,
        Equipment
    }

    public override void UpdateInformation(object _data)
    {
        userData = GameData.Instance.userData;
        inventoryService = new InventoryService(userData);
        shopService = new ShopService(userData, inventoryService);

        dropdownCategory.ClearOptions();
        dropdownCategory.AddOptions(new List<string> { "Crop & Animal", "Land", "Worker", "Equipment" });
        dropdownCategory.onValueChanged.AddListener(OnCategoryChanged);

        OnCategoryChanged(0);
    }

    private void OnCategoryChanged(int index)
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        switch ((Category)index)
        {
            case Category.CropAnimal:
                foreach (var kv in FarmEntityConfigLoader.All)
                {
                    var cfg = kv.Value;
                    CreateItem(cfg.DisplayName, cfg.BuyPrice, () =>
                    {
                        if (shopService.BuyEntity(cfg.Id))
                        {
                            ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_MONEY);
                            Debug.Log($"✅ Mua {cfg.Id} thành công");
                        }
                        else
                            Debug.LogWarning("❌ Không đủ gold hoặc lỗi mua");
                    });
                }
                break;

            case Category.Land:
                foreach (var kv in LandConfigLoader.All)
                {
                    var cfg = kv.Value;
                    CreateItem(cfg.DisplayName, cfg.Price, () =>
                    {
                        if (shopService.BuyLand(cfg.Id))
                        {
                            Debug.Log($"✅ Mua Land {cfg.Id} thành công");
                            ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_MONEY);
                            ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_LAND);

                        }
                        else
                            Debug.LogWarning("❌ Không đủ gold hoặc lỗi mua");
                    });
                }
                break;

            case Category.Worker:
                foreach (var kv in WorkerConfigLoader.All)
                {
                    var cfg = kv.Value;
                    CreateItem(cfg.DisplayName, cfg.Price, () =>
                    {
                        if (shopService.HireWorker(cfg.Id))
                        {
                            ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_MONEY);
                            Debug.Log($"✅ Thuê {cfg.Id} thành công");

                        }
                        else
                            Debug.LogWarning("❌ Không đủ gold hoặc lỗi thuê");
                    });
                }
                break;

            case Category.Equipment:
                foreach (var kv in EquipmentConfigLoader.All)
                {
                    var cfg = kv.Value;
                    CreateItem(cfg.DisplayName, cfg.Price, () =>
                    {
                        if (shopService.BuyEquipment(cfg.Id))
                        {
                            ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_MONEY);
                            Debug.Log($"✅ Mua Equipment {cfg.Id} thành công");
                        }
                        else
                            Debug.LogWarning("❌ Không đủ gold hoặc lỗi mua thiết bị");
                    });
                }
                break;
        }
        itemPrefab.SetActive(false);
    }

    private void CreateItem(string name, int price, System.Action onBuy)
    {
        var go = Instantiate(itemPrefab, contentRoot);
        ShopItem texts = go.GetComponent<ShopItem>();
        texts.gameObject.SetActive(true);
        texts.InitData(name, price.ToString() + "Gold", onBuy);
    }
}