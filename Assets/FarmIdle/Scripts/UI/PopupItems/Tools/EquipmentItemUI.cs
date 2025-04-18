using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Data;

public class EquipmentItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI boostText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button upgradeButton;

    private EquipmentConfigData config;

    public void Set(EquipmentConfigData config, int currentLevel, System.Action onUpgrade)
    {
        this.config = config;
        nameText.text = config.DisplayName;
        levelText.text = $"Lv {currentLevel}/{config.MaxLevel}";
        boostText.text = $"+{config.BoostPercent}%";

        upgradeButton.interactable = currentLevel < config.MaxLevel;
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() => onUpgrade?.Invoke());
    }
}