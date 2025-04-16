using Data;
using CoreGamePlay;

namespace Service
{
    public class UpgradeService
    {
        private const int UpgradeCost = 500;

        private readonly UserData _userData;
        private readonly InventoryService _inventoryService;

        public UpgradeService(UserData userData, InventoryService inventoryService)
        {
            _userData = userData;
            _inventoryService = inventoryService;
        }

        public int GetEquipmentLevel()
        {
            return _userData.Equipment.Level;
        }

        public double GetCurrentYieldMultiplier()
        {
            return _userData.Equipment.GetYieldMultiplier();
        }

        public bool CanUpgrade()
        {
            return _inventoryService.HasEnoughGold(UpgradeCost);
        }

        public bool TryUpgrade()
        {
            if (!CanUpgrade()) return false;

            _inventoryService.SpendGold(UpgradeCost);
            _userData.Equipment.Upgrade();
            return true;
        }
    }
}