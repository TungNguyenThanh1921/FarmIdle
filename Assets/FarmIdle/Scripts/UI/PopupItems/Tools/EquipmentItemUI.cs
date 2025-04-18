using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Data;
using System;

public class EquipmentItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text boostText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Button upgradeButton;

    private EquipmentConfigData config;
    public void Set(EquipmentConfigData config,double boostPercent, int currentLevel, int upgradeCost, Action onClick)
    {
        nameText.text = config.DisplayName;
        levelText.text = $"Level: {currentLevel}";
        boostText.text = $"Boost: {boostPercent}%";
        upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = $"{upgradeCost}";
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() => onClick?.Invoke());
    }
}