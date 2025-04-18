using System.Collections.Generic;
using System.Linq;
using Data;

namespace Service
{
    public class InventoryService
    {
        private readonly UserData _userData;

        public InventoryService(UserData userData)
        {
            _userData = userData;
        }

        // VÀNG 💰
        public int GetGold() => _userData.Gold;

        public void AddGold(int amount)
        {
            _userData.Gold += amount;
        }
        public Dictionary<string, int> GetAllItems()
        {
            return new Dictionary<string, int>(_userData.Inventory);
        }
        public bool HasEnoughGold(int amount) => _userData.Gold >= amount;

        public bool SpendGold(int amount)
        {
            if (!HasEnoughGold(amount)) return false;
            _userData.Gold -= amount;
            return true;
        }

        // TÀI NGUYÊN 🌽🥛🫐
        public int GetItemAmount(string itemName)
        {
            if (_userData.Inventory.TryGetValue(itemName, out int amount))
                return amount;
            return 0;
        }

        public void AddItem(string itemName, int amount)
        {
            if (!_userData.Inventory.ContainsKey(itemName))
                _userData.Inventory[itemName] = 0;

            _userData.Inventory[itemName] += amount;
        }

        public bool HasEnoughItem(string itemName, int required)
        {
            return GetItemAmount(itemName) >= required;
        }

        public bool SpendItem(string itemName, int amount)
        {
            if (!HasEnoughItem(itemName, amount)) return false;

            _userData.Inventory[itemName] -= amount;
            return true;
        }
        public int SellAllHarvestedProducts()
        {
            int totalGold = 0;
            var keysToRemove = new List<string>();

            foreach (var kvp in _userData.Inventory)
            {
                string itemId = kvp.Key;
                int quantity = kvp.Value;

                if (itemId.EndsWith("Seed")) continue;

                // Tìm config theo ProductId
                var cfg = FarmEntityConfigLoader.All.Values
                    .FirstOrDefault(c => c.ProductId == itemId);

                if (cfg != null)
                {
                    int goldEarned = cfg.SellPrice * quantity;
                    totalGold += goldEarned;
                    keysToRemove.Add(itemId);
                }
            }

            foreach (var id in keysToRemove)
            {
                _userData.Inventory.Remove(id);
            }

            _userData.Gold += totalGold;
            return totalGold;
        }
    }
}