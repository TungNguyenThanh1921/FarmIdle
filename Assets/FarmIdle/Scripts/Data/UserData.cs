using System;
using System.Collections.Generic;
using CoreBase;
using CoreGamePlay;
using Data;

namespace Data
{
    public class UserData
    {
        public int Gold = 1000;
        public Dictionary<string, int> Inventory = new();
        public List<EquipmentEntity> Equipments = new();
        public List<FarmSlot> Slots = new();
        public List<WorkerEntity> Workers = new();
        public ITimeProvider LastExitTime;

        public void InitData()
        {
            Gold = 1000;
            Inventory = new Dictionary<string, int>();
            Inventory["TomatoSeed"] = 10;
            Inventory["BlueberrySeed"] = 10;
            Inventory["CowSeed"] = 10;

            Equipments = new List<EquipmentEntity>();
            Equipments.Add(new EquipmentEntity(EquipmentConfigLoader.GetDefault()));
            Slots = new List<FarmSlot>();
            for (int i = 0; i < 3; i++)
            {
                var landEntity = new LandEntity(LandConfigLoader.GetDefault());
                Slots.Add(new FarmSlot(landEntity));
            }
            Workers = new List<WorkerEntity>();
            Workers.Add(new WorkerEntity(WorkerConfigLoader.GetDefault()));

            LastExitTime = null;
        }
        public void SaveExitTime(ITimeProvider time)
        {
            LastExitTime = time;
        }

        public void ClearData()
        {
            Gold = 0;
            Inventory = new();
            Equipments = new();
            Slots = new();
            Workers = new();
        }
    }
}