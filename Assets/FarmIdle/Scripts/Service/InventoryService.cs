using System.Collections.Generic;
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
     
    }
}