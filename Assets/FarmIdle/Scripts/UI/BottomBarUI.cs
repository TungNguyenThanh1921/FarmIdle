using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomBarUI : MonoBehaviour
{
    [SerializeField] private Button Inventory;
    [SerializeField] private Button Shop;
    [SerializeField] private Button Tools;
    void Start()
    {

        Inventory.onClick.RemoveAllListeners();
        Inventory.onClick.AddListener(() =>
        {
            OpenInventory();
        });
        Shop.onClick.RemoveAllListeners();
        Shop.onClick.AddListener(() =>
        {
            OpenShop();
        });
        Tools.onClick.RemoveAllListeners();
        Tools.onClick.AddListener(() =>
        {
            OpenTools();
        });
    }

    private void OpenInventory()
    {
        PopupManager.Instance.OpenPopup(PopupIDs.Inventory);
    }
    private void OpenTools()
    {
        PopupManager.Instance.OpenPopup(PopupIDs.Equipment);
    }
    private void OpenShop()
    {
        PopupManager.Instance.OpenPopup(PopupIDs.Shop);
    }
}
