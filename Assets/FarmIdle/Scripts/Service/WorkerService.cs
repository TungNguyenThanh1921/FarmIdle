using System;
using System.Collections.Generic;
using Data;
using CoreGamePlay;
using CoreBase;
using Observer;
using UnityEngine;

namespace Service
{
    public class WorkerService
    {
        private readonly UserData userData;
        private readonly InventoryService inventory;
        private readonly FarmService farmService;
        private readonly ITimeProvider timeProvider;

        private static readonly TimeSpan TaskDuration = TimeSpan.FromMinutes(2);

        public WorkerService(UserData userData, InventoryService inventory, FarmService farmService, ITimeProvider timeProvider)
        {
            this.userData = userData;
            this.inventory = inventory;
            this.farmService = farmService;
            this.timeProvider = timeProvider;

            ProcessOfflineWork();       // Xử lý thời gian offline
        }

        public void RebindAssignedSlots()
        {
            foreach (var worker in userData.Workers)
            {
                if (worker.IsBusy && worker.WorkingSlotIndex.HasValue)
                {
                    int index = worker.WorkingSlotIndex.Value;
                    if (index >= 0 && index < userData.Slots.Count)
                    {
                        var slot = userData.Slots[index];
                        if (!worker.IsTaskDone(timeProvider))
                        {
                            slot.AssignedWorker = worker;
                        }
                    }
                }
            }
        }

        public void Tick()
        {
            foreach (var worker in userData.Workers)
            {
                // ✅ Nếu đang làm và đã xong task
                if (worker.IsBusy && worker.IsTaskDone(timeProvider))
                {
                    CompleteWorkerTask(worker);
                }

                // ✅ Nếu đang rảnh → tìm slot cần làm
                if (!worker.IsBusy)
                {
                    AssignTaskToWorker(worker);
                }
            }
        }

        private void CompleteWorkerTask(WorkerEntity worker)
        {
            int slotIndex = worker.WorkingSlotIndex ?? -1;
            if (slotIndex >= 0 && slotIndex < userData.Slots.Count)
            {
                var slot = userData.Slots[slotIndex];

                if (slot.CanHarvestAny(timeProvider))
                {
                    farmService.HarvestSlot(slotIndex);
                }
                else if (!slot.IsFull && !string.IsNullOrEmpty(slot.LockedType))
                {
                    farmService.TryPlantAtSlot(slotIndex);
                }

                slot.AssignedWorker = null;
            }

            worker.CompleteTask();
            ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_MONEY);
            ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_LAND);
        }

        private void AssignTaskToWorker(WorkerEntity worker)
        {
            // 👉 Bước 1: Dọn dẹp slot bị gán nhưng không còn công việc
            foreach (var slot in userData.Slots)
            {
                bool cannotHarvest = !slot.CanHarvestAny(timeProvider);
                bool cannotPlant = slot.IsFull || string.IsNullOrEmpty(slot.LockedType) || !inventory.HasEnoughItem(slot.LockedType + "Seed", 1);

                if (slot.AssignedWorker != null && cannotHarvest && cannotPlant)
                {
                    slot.AssignedWorker = null;
                    ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_LAND);
                    ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_MONEY);
                }
            }

            // 👉 Bước 2: Gán cho slot cần thu hoạch trước
            for (int i = 0; i < userData.Slots.Count; i++)
            {
                var slot = userData.Slots[i];
                if (slot.AssignedWorker == null && slot.CanHarvestAny(timeProvider))
                {
                    slot.AssignedWorker = worker;
                    worker.AssignTask(timeProvider, i);
                    ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_LAND);
                    ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_MONEY);
                    return;
                }
            }

            // 👉 Bước 3: Gán cho slot có thể trồng (phải đủ điều kiện)
            for (int i = 0; i < userData.Slots.Count; i++)
            {
                if (farmService.CanPlant(i) && inventory.HasEnoughItem(userData.Slots[i].LockedType + "Seed", FarmEntityConfigLoader.All[userData.Slots[i].LockedType].SeedRequired))
                {
                    var slot = userData.Slots[i];
                    if (slot.AssignedWorker == null)
                    {
                        slot.AssignedWorker = worker;
                        worker.AssignTask(timeProvider, i);
                        ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_LAND);
                        ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_MONEY);
                        return;
                    }
                }
            }
        }

        private void ProcessOfflineWork()
        {
            DateTime now = timeProvider.Now;
            DateTime lastExit = userData.LastExitTime.Now;
            TimeSpan delta = now - lastExit;

            int totalJobs = (int)(delta.TotalMinutes / 2);
            if (totalJobs <= 0) return;

            int jobCount = 0;

            for (int i = 0; i < totalJobs; i++)
            {
                foreach (var worker in userData.Workers)
                {
                    if (jobCount >= totalJobs) return;

                    if (TryOfflineHarvest()) { jobCount++; continue; }
                    if (TryOfflinePlant()) { jobCount++; continue; }
                }
            }
        }

        private bool TryOfflineHarvest()
        {
            for (int i = 0; i < userData.Slots.Count; i++)
            {
                if (userData.Slots[i].CanHarvestAny(timeProvider))
                {
                    farmService.HarvestSlot(i);
                    return true;
                }
            }
            return false;
        }

        private bool TryOfflinePlant()
        {
            for (int i = 0; i < userData.Slots.Count; i++)
            {
                if (farmService.TryPlantAtSlot(i))
                {
                    return true;
                }
            }
            return false;
        }

        public int IdleWorkerCount() => userData.Workers.FindAll(w => w.IsIdle(timeProvider)).Count;

        public int IdleWorkerCount(ITimeProvider time) => userData.Workers.FindAll(w => w.IsIdle(time)).Count;
    }
}