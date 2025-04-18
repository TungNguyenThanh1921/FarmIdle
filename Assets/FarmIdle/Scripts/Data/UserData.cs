using System;
using System.Collections.Generic;
using CoreGamePlay;
using Data;

namespace Data
{
    public class UserData
    {
        public int Gold = 1000;
        public Dictionary<string, int> Inventory = new();
        public EquipmentEntity Equipment;
        public List<FarmSlot> Slots = new();
        public List<WorkerEntity> Workers = new();
        public System.DateTime LastExitTime;

        public void InitData()
        {
            Gold = 1000;
            Inventory = new Dictionary<string, int>();
            Inventory["TomatoSeed"] = 10;
            Inventory["BlueberrySeed"] = 10;
            Inventory["CowSeed"] = 10;

            Equipment = new EquipmentEntity(EquipmentConfigLoader.GetDefault());
            Slots = new List<FarmSlot>();
            for (int i = 0; i < 3; i++)
            {
                var landEntity = new LandEntity(LandConfigLoader.GetDefault());
                Slots.Add(new FarmSlot(landEntity));
            }
            Workers = new List<WorkerEntity>();
            Workers.Add(new WorkerEntity(WorkerConfigLoader.GetDefault()));

            LastExitTime = DateTime.Now;
        }

        public void ClearData()
        {
            Gold = 0;
            Inventory = new();
            Equipment = null;
            Slots = new();
            Workers = new();
        }
    }
}