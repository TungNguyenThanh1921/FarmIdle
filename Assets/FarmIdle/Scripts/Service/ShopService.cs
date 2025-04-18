using CoreGamePlay;
using Data;
using Service;

public class ShopService
{
    private readonly UserData _userData;
    private readonly InventoryService _inventory;

    public ShopService(UserData userData, InventoryService inventory)
    {
        _userData = userData;
        _inventory = inventory;
    }

    public bool BuyEntity(string entityId)
    {
        if (!FarmEntityConfigLoader.All.TryGetValue(entityId, out var config))
            return false;

        int cost = config.BuyPrice;
        int amount = config.IsBulk ? 10 : 1;

        string itemId = config.Id + "Seed";

        if (!_inventory.SpendGold(cost)) return false;

        _inventory.AddItem(itemId, amount);
        return true;
    }

    public bool BuyLand(string landId)
    {
        if (!LandConfigLoader.All.TryGetValue(landId, out var config))
            return false;

        if (!_inventory.SpendGold(config.Price)) return false;

        _userData.Slots.Add(new FarmSlot(new LandEntity(config)));
        return true;
    }

    public bool HireWorker(string workerId)
    {
        if (!WorkerConfigLoader.All.TryGetValue(workerId, out var config))
            return false;

        if (!_inventory.SpendGold(config.Price)) return false;

        _userData.Workers.Add(new WorkerEntity(config));
        return true;
    }

    public bool BuyEquipment(string equipId)
    {
        if (!EquipmentConfigLoader.All.TryGetValue(equipId, out var config))
            return false;

        if (!_inventory.SpendGold(config.Price)) return false;

        // Thêm thiết bị mới vào danh sách
        _userData.Equipments.Add(new EquipmentEntity(config));
        return true;
    }
}