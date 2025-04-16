using System;
using System.Collections.Generic;
using CoreGamePlay;
using CoreBase;

namespace Service
{
    public class FarmService
    {
        public List<FarmSlot> Slots { get; private set; } = new();
        public List<WorkerEntity> Workers { get; private set; } = new();
        private readonly ITimeProvider time;

        public FarmService(int initialSlots, int initialWorkers, ITimeProvider timeProvider)
        {
            time = timeProvider;
            for (int i = 0; i < initialSlots; i++) Slots.Add(new FarmSlot());
            for (int i = 0; i < initialWorkers; i++) Workers.Add(new GeneralWorker());
        }

        public bool TryAssignEntity(int slotIndex, FarmEntity entity)
        {
            if (slotIndex < 0 || slotIndex >= Slots.Count) return false;
            if (!Slots[slotIndex].IsEmpty) return false;

            Slots[slotIndex].AssignEntity(entity);
            return true;
        }

        public int HarvestSlot(int slotIndex)
        {
            var slot = Slots[slotIndex];
            return slot.Entity?.Harvest(time.Now) ?? 0;
        }

        public void AssignWorkerTo(Action task, TimeSpan duration)
        {
            var idle = Workers.Find(w => !w.IsBusy);
            if (idle != null)
            {
                idle.AssignTask(duration, time.Now);
                task.Invoke();
                idle.CompleteTask();
            }
        }

        public List<FarmEntity> GetAllActiveEntities()
        {
            List<FarmEntity> list = new();
            foreach (var slot in Slots)
            {
                if (!slot.IsEmpty) list.Add(slot.Entity);
            }
            return list;
        }
    }
}
