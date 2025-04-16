using CoreGamePlay;
using Data;

namespace Service
{
    public class ShopService
    {
        private readonly UserData _userData;
        private readonly InventoryService _inventory;

        public ShopService(UserData userData, InventoryService inventoryService)
        {
            _userData = userData;
            _inventory = inventoryService;
        }

        // Giá cố định theo đề bài
        private const int TomatoSeedCost = 30;
        private const int BlueberrySeedCost = 50;
        private const int StrawberrySeedCostPer10 = 400;
        private const int CowCost = 100;
        private const int LandCost = 500;
        private const int WorkerCost = 500;

        public bool BuyTomatoSeed()
        {
            if (!_inventory.SpendGold(TomatoSeedCost)) return false;
            _inventory.AddItem("TomatoSeed", 1);
            return true;
        }

        public bool BuyBlueberrySeed()
        {
            if (!_inventory.SpendGold(BlueberrySeedCost)) return false;
            _inventory.AddItem("BlueberrySeed", 1);
            return true;
        }

        public bool BuyStrawberrySeed()
        {
            if (!_inventory.SpendGold(StrawberrySeedCostPer10)) return false;
            _inventory.AddItem("StrawberrySeed", 10);
            return true;
        }

        public bool BuyCow()
        {
            if (!_inventory.SpendGold(CowCost)) return false;
            _inventory.AddItem("Cow", 1); // Hoặc lưu vào nơi riêng nếu bạn phân biệt seed vs animal
            return true;
        }

        public bool BuyLand()
        {
            if (!_inventory.SpendGold(LandCost)) return false;
            _userData.Slots.Add(new FarmSlot());
            return true;
        }

        public bool HireWorker()
        {
            if (!_inventory.SpendGold(WorkerCost)) return false;
            _userData.Workers.Add(new GeneralWorker());
            return true;
        }
    }
}