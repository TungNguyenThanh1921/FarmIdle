using Data;

namespace Service
{
    public class SellService
    {
        private readonly InventoryService _inventory;

        // Giá bán cố định theo đề bài
        private const int TomatoPrice = 5;
        private const int BlueberryPrice = 8;
        private const int MilkPrice = 15;

        public SellService(InventoryService inventory)
        {
            _inventory = inventory;
        }

        public bool TrySell(string itemName, int amount)
        {
            if (!_inventory.HasEnoughItem(itemName, amount))
                return false;

            int goldEarned = GetSellPrice(itemName) * amount;
            _inventory.SpendItem(itemName, amount);
            _inventory.AddGold(goldEarned);
            return true;
        }

        public int GetSellPrice(string itemName)
        {
            return itemName switch
            {
                "Tomato" => TomatoPrice,
                "Blueberry" => BlueberryPrice,
                "Milk" => MilkPrice,
                _ => 0
            };
        }
    }
}