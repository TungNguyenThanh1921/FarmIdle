using System.Collections.Generic;
using CoreGamePlay;

namespace Data
{
    public class UserData
    {
        public void InitData()
        {
            Gold = 1000;


            // Khởi tạo mặc định nếu cần
            for (int i = 0; i < 3; i++)
            {
                Slots.Add(new FarmSlot());
            }

            for (int i = 0; i < 1; i++)
            {
                Workers.Add(new GeneralWorker());
            }

            // Inventory mẫu
            Inventory["TomatoSeed"] = 10;
            Inventory["BlueberrySeed"] = 10;
            Inventory["Cow"] = 0;
        }
        public void ClearData()
        {
            Gold = 0;
            Inventory = new();
            Equipment = new();
            Slots = new();
            Workers = new();
        }
        public int Gold = 1000;

        public Dictionary<string, int> Inventory = new();

        public FarmEquipment Equipment = new();

        public List<FarmSlot> Slots = new();

        public List<WorkerEntity> Workers = new();

        public System.DateTime LastExitTime;
    }
}