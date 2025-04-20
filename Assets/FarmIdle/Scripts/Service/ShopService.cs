using System.Linq;
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

        // Nếu tất cả slot đã có worker  không thuê nữa
        bool allSlotsAssigned = _userData.Slots
            .Where(s => !string.IsNullOrEmpty(s.LockedType))
            .All(s => _userData.Workers.Any(w => w.WorkingSlotIndex == _userData.Slots.IndexOf(s)));

        if (allSlotsAssigned)
            return false;

        if (!_inventory.SpendGold(config.Price)) return false;

        // Tìm slot đầu tiên chưa có worker để gán
        for (int i = 0; i < _userData.Slots.Count; i++)
        {
            var slot = _userData.Slots[i];
            if (!_userData.Workers.Any(w => w.WorkingSlotIndex == i))
            {
                var worker = new WorkerEntity(config);
                worker.AssignSlot(i);
                _userData.Workers.Add(worker);
                return true;
            }
        }

        return false; // fallback nếu không tìm được slot
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