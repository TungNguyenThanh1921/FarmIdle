using Data;
using Service;

public class EquipmentService
{
    private readonly UserData _userData;
    private readonly InventoryService _inventory;

    public EquipmentService(UserData data, InventoryService inventory)
    {
        _userData = data;
        _inventory = inventory;
    }

    public bool TryUpgrade(string id)
    {
        if (_userData.Equipment == null) return false;
        if (_userData.Equipment.Config.Id != id) return false;

        if (_userData.Equipment.Level >= _userData.Equipment.Config.MaxLevel)
            return false;

        int upgradeCost = CalculateUpgradeCost(_userData.Equipment.Config, _userData.Equipment.Level);
        if (!_inventory.SpendGold(upgradeCost)) return false;

        _userData.Equipment.Upgrade();
        return true;
    }

    private int CalculateUpgradeCost(EquipmentConfigData config, int currentLevel)
    {
        // Ví dụ: upgrade từ level 1 lên 2 → cost = BasePrice * 2
        return config.Price * currentLevel;
    }
}