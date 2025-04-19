using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using CoreBase;

namespace CoreGamePlay
{
    public class FarmSlot
    {
        [JsonProperty] public LandEntity Land { get; private set; }
        [JsonProperty] public string LockedType { get; private set; }
        [JsonProperty] public List<FarmEntity> Entities { get; private set; } = new();

        [JsonIgnore] public Action OnEntitiesChanged;
        [JsonIgnore] public WorkerEntity AssignedWorker { get; set; } = null;

        public bool IsBeingWorked => AssignedWorker != null;
        public bool IsFull => Entities.Count >= Land.Config.SlotCount;
        public bool IsEmpty => Entities.Count == 0;

        public FarmSlot(LandEntity land)
        {
            Land = land;
        }

        public bool CanAssign(string type)
        {
            return LockedType == null || LockedType == type;
        }

        public int TotalPlantedSlot() => Entities.Count;

        public bool TryAddOneEntity(string type, Func<FarmEntity> factory)
        {
            if (!CanAssign(type)) return false;
            if (IsFull) return false;
            Entities.Add(factory());
            return true;
        }

        public int HarvestAll(ITimeProvider time, List<EquipmentEntity> equipments, out int removedCount)
        {
            removedCount = 0;
            int totalYield = 0;

            float totalMultiplier = 1f;
            if (equipments != null)
            {
                foreach (var equip in equipments)
                {
                    totalMultiplier *= (float)equip.GetYieldMultiplier();
                }
            }

            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                var entity = Entities[i];
                int baseAmount = entity.GetAvailableYield(time);
                if (baseAmount <= 0) continue;
                int bonusAmount = (int)Math.Floor(baseAmount * totalMultiplier);

                if (Land != null && Land.Config.YieldBuffPercent > 0)
                {
                    bonusAmount = (int)(bonusAmount * (1 + Land.Config.YieldBuffPercent / 100f));
                }

                entity.Harvest(time);
                totalYield += bonusAmount;

                if (entity.Yielded >= entity.MaxYield)
                {
                    Entities.RemoveAt(i);
                    removedCount++;
                }
            }

            return totalYield;
        }

        public string GetProductName()
        {
            return LockedType switch
            {
                "Cow" => "Milk",
                _ => LockedType
            };
        }

        public bool AssignRole(string type)
        {
            if (LockedType != null) return false;
            if (!CanAssign(type)) return false;

            LockedType = type;
            return true;
        }

        public bool CanHarvestAny(ITimeProvider time)
        {
            return Entities.Any(e => e.CanHarvest(time));
        }

        public string GetRemainingTimeText(ITimeProvider time)
        {
            int removed = Entities.RemoveAll(e => e.IsExpired(time) && e.CheckDestroyItem(time));
            if (removed > 0)
            {
                OnEntitiesChanged?.Invoke();
            }

            var rotEntity = Entities
                .Where(e => e.IsExpired(time))
                .OrderBy(e => e.GetSecondsUntilRot(time))
                .FirstOrDefault();

            if (rotEntity != null)
            {
                int rotSeconds = rotEntity.GetSecondsUntilRot(time);
                var rotSpan = TimeSpan.FromSeconds(rotSeconds);
                return $"Sắp héo! {(int)rotSpan.TotalHours:00}:{rotSpan.Minutes:00}:{rotSpan.Seconds:00}";
            }

            var nextEntity = Entities
                .Where(e => !e.IsExpired(time))
                .OrderBy(e => e.RemainingLifeSeconds(time))
                .FirstOrDefault();

            if (nextEntity != null)
            {
                int remaining = nextEntity.RemainingLifeSeconds(time);
                var remainSpan = TimeSpan.FromSeconds(remaining);
                return $"Còn {(int)remainSpan.TotalHours:00}:{remainSpan.Minutes:00}:{remainSpan.Seconds:00}";
            }

            return "";
        }

        public int GetTotalYielded() => Entities.Sum(e => e.Yielded);
        public int GetTotalMaxYield() => Entities.Sum(e => e.MaxYield);

        public bool IsDead(ITimeProvider time)
        {
            return Entities.All(e => e.IsExpired(time));
        }
    }
}