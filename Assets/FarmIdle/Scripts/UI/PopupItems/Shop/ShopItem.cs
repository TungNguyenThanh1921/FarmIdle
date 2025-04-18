using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] TMP_Text name;
    [SerializeField] TMP_Text price;
    [SerializeField] Button buy;
    public void InitData(string name, string price, System.Action onBuy)
    {
        this.name.text = name;
        this.price.text = price;
        buy.onClick.RemoveAllListeners();
        buy.onClick.AddListener(() => onBuy?.Invoke());
    }
}
