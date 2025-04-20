using System;
using System.Collections.Generic;
using Data;
using CoreGamePlay;
using CoreBase;
using Observer;
using UnityEngine;
using System.Linq;

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

        }

        public void RebindAssignedSlots()
        {
            foreach (var worker in userData.Workers)
            {
                if (worker.WorkingSlotIndex.HasValue)
                {
                    int index = worker.WorkingSlotIndex.Value;
                    if (index >= 0 && index < userData.Slots.Count)
                    {
                        var slot = userData.Slots[index];
                        slot.AssignedWorker = worker;
                    }
                }
            }
        }

        public void Tick()
        {
            foreach (var worker in userData.Workers)
            {
                //  Nếu đang làm và đã xong task
                if (worker.IsBusy && worker.IsTaskDone(timeProvider))
                {
                    CompleteWorkerTask(worker);
                }

                //  Nếu đang rảnh → tìm slot cần làm
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
            }

            worker.CompleteTask();
            ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_MONEY);
            ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_LAND);
        }

        private void AssignTaskToWorker(WorkerEntity worker)
        {
            int slotIndex = worker.WorkingSlotIndex ?? -1;
            if (slotIndex < 0 || slotIndex >= userData.Slots.Count) return;

            var slot = userData.Slots[slotIndex];

            if (slot.AssignedWorker != null && slot.AssignedWorker != worker) return;

            // Gán worker nếu có việc để làm
            if (slot.CanHarvestAny(timeProvider) || (farmService.CanPlant(slotIndex) && inventory.HasEnoughItem(slot.LockedType + "Seed", 1)))
            {
                slot.AssignedWorker = worker;
                worker.AssignTask(timeProvider, slotIndex);
                ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_LAND);
                ObserverManager.Instance.Notify(EventKeys.UI.UPDATE_MONEY);
            }
        }
        public void ProcessOfflineWork()
        {
            if (userData.Workers.Count > 0)
                CheckOfflineWithWorker();
            else
                CheckOfflineWithoutWorker();
        }
        public void CheckOfflineWithoutWorker()
        {
            DateTime now = timeProvider.Now;

            foreach (var slot in userData.Slots)
            {
                for (int i = slot.Entities.Count - 1; i >= 0; i--)
                {
                    var entity = slot.Entities[i];

                    DateTime endYield = entity.BornAt.AddSeconds(entity.YieldIntervalSeconds * entity.MaxYield);
                    DateTime expireTime = endYield.AddHours(1);

                    if (now > expireTime)
                    {
                        slot.Entities.RemoveAt(i); // cây đã chết
                    }
                }
            }
        }
        public void CheckOfflineWithWorker()
        {
            DateTime now = timeProvider.Now;
            DateTime lastExit = userData.LastExitTime.Now;
            double offlineSeconds = (now - lastExit).TotalSeconds;
            if (offlineSeconds <= 0) return;

            foreach (var worker in userData.Workers)
            {
                int slotIndex = worker.WorkingSlotIndex ?? -1;
                if (slotIndex < 0 || slotIndex >= userData.Slots.Count) continue;

                var slot = userData.Slots[slotIndex];
                if (string.IsNullOrEmpty(slot.LockedType)) continue;

                var cfg = FarmEntityConfigLoader.All[slot.LockedType];
                int yieldInterval = cfg.YieldInterval;
                int maxYield = cfg.MaxYield;
                int slotLimit = slot.Land.Config.SlotCount;
                int seedCost = cfg.SeedRequired;
                string seedId = cfg.Id + "Seed";

                int jobLimit = (int)(offlineSeconds / 120);
                int jobsUsed = 0;
                double usedTime = 0;

                while (jobsUsed < jobLimit && usedTime < offlineSeconds)
                {
                    // 1. Trồng nếu còn slot trống
                    while (!slot.IsFull && inventory.HasEnoughItem(seedId, seedCost) && jobsUsed < jobLimit)
                    {
                        slot.TryAddOneEntity(cfg.Id, () => FarmEntityFactory.Create(cfg.Id, lastExit));
                        inventory.SpendItem(seedId, seedCost);
                        jobsUsed++;
                    }

                    // 2. Nếu slot đầy → Thu hoạch toàn bộ nếu cây đủ sản lượng
                    bool hasHarvested = false;
                    int totalYield = 0;

                    foreach (var entity in slot.Entities.ToList())
                    {
                        int remaining = entity.MaxYield - entity.Yielded;
                        int canProduce = (int)((offlineSeconds - usedTime) / yieldInterval);
                        int yieldCount = Math.Min(remaining, canProduce);

                        if (yieldCount > 0)
                        {
                            entity.AddYield(yieldCount);
                            usedTime += yieldCount * yieldInterval;
                            totalYield += yieldCount;

                            if (entity.Yielded >= entity.MaxYield)
                                slot.Entities.Remove(entity);
                        }
                    }

                    if (totalYield > 0)
                    {
                        int finalAmount = (int)(totalYield * (1 + userData.Equipments[0].GetYieldMultiplier()));
                        inventory.AddItem(cfg.ProductId, finalAmount);
                        jobsUsed++;
                        hasHarvested = true;
                    }

                    if (!hasHarvested) break; // Nếu không còn gì để làm thì thoát
                }
            }
        }

        public int IdleWorkerCount() => userData.Workers.FindAll(w => w.IsAssignedToSlot()).Count;

    }
}