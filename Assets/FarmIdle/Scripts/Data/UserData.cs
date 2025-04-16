using System.Collections.Generic;
using CoreGamePlay;

namespace Data
{
    public class UserData
    {
        public int Gold = 1000;

        public Dictionary<string, int> Inventory = new();

        public FarmEquipment Equipment = new();

        public List<FarmSlot> Slots = new();

        public List<WorkerEntity> Workers = new();

        public System.DateTime LastExitTime;
    }
}