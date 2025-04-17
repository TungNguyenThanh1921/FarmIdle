using System;
using System.Linq;
using CoreBase;
using Data;

namespace Service
{
    public class FarmService
    {
        private readonly UserData userData;
        private readonly InventoryService inventory;
        private readonly ITimeProvider time;

        public FarmService(UserData userData, InventoryService inventory, ITimeProvider timeProvider)
        {
            this.userData = userData;
            this.inventory = inventory;
            this.time = timeProvider;
        }

        /// <summary>
        /// Trồng tự động vào mảnh đầu tiên hợp lệ (nếu dùng)
        /// </summary>
        public bool TryPlant()
        {
            for (int i = 0; i < userData.Slots.Count; i++)
            {
                if (TryPlantAtSlot(i)) return true;
            }

            UnityEngine.Debug.LogWarning("❌ Không tìm thấy slot hợp lệ để trồng.");
            return false;
        }

        /// <summary>
        /// Trồng 1 cây vào đúng mảnh chỉ định
        /// </summary>
        public bool TryPlantAtSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= userData.Slots.Count)
                return false;

            var slot = userData.Slots[slotIndex];
            if (slot.IsFull || string.IsNullOrEmpty(slot.LockedType)) return false;

            string cropId = slot.LockedType;
            if (!FarmEntityConfigLoader.All.TryGetValue(cropId, out var cfg)) return false;

            string seedId = cropId + "Seed";
            if (!inventory.HasEnoughItem(seedId, cfg.SeedRequired)) return false;

            inventory.SpendItem(seedId, cfg.SeedRequired);
            return slot.TryAddOneEntity(cropId, () => FarmEntityFactory.Create(cropId, time.Now));
        }

        /// <summary>
        /// Thu hoạch toàn bộ mảnh đất
        /// </summary>
        public int HarvestSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= userData.Slots.Count)
                return 0;

            var slot = userData.Slots[slotIndex];

            if (!slot.CanHarvestAny(time.Now)) return 0;

            int removed;
            int harvested = slot.HarvestAll(time.Now, userData.Equipment, out removed);

            if (harvested > 0)
            {
                string product = slot.GetProductName();
                inventory.AddItem(product, harvested);
            }

            return harvested;
        }
        public bool TryAssignRoleToSlot(int slotIndex, string type)
        {
            if (slotIndex < 0 || slotIndex >= userData.Slots.Count)
                return false;

            var slot = userData.Slots[slotIndex];
            return slot.AssignRole(type);
        }
    }
}