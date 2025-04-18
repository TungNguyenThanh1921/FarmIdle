using System.Linq;
using Data;
using CoreGamePlay;
using System.Collections.Generic;

namespace Service
{
    public class EquipmentService
    {
        private readonly UserData _userData;
        private readonly InventoryService _inventory;

        public EquipmentService(UserData data, InventoryService inventory)
        {
            _userData = data;
            _inventory = inventory;
        }

        /// <summary>
        /// Nâng cấp thiết bị với id tương ứng (nếu tìm thấy và đủ điều kiện)
        /// </summary>
        public bool TryUpgrade(string id)
        {
            var equip = _userData.Equipments.FirstOrDefault(e => e.Config.Id == id);
            if (equip == null) return false;

            if (equip.Level >= equip.Config.MaxLevel)
                return false;

            int upgradeCost = CalculateUpgradeCost(equip.Config, equip.Level);
            if (!_inventory.SpendGold(upgradeCost)) return false;

            equip.Upgrade();
            return true;
        }

        /// <summary>
        /// Tính giá nâng cấp cho thiết bị
        /// </summary>
        public static int CalculateUpgradeCost(EquipmentConfigData config, int currentLevel)
        {
            return config.Price * currentLevel;
        }

        /// <summary>
        /// Lấy danh sách thiết bị hiện có
        /// </summary>
        public List<EquipmentEntity> GetAllEquipments()
        {
            return _userData.Equipments;
        }
    }
}