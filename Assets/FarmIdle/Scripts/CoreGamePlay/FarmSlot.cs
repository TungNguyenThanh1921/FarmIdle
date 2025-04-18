using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CoreGamePlay
{
    public class FarmSlot
    {
        [JsonProperty]
        public LandEntity Land { get; private set; }
        [JsonProperty]
        public string LockedType { get; private set; }
        [JsonProperty]
        public List<FarmEntity> Entities { get; private set; } = new();

        public bool IsFull => Entities.Count >= Land.Config.SlotCount;

        public bool IsEmpty => Entities.Count == 0;
        // Callback để thông báo UI cần update lại
        [JsonIgnore]
        public Action OnEntitiesChanged;
        public FarmSlot(LandEntity land)
        {
            Land = land;
        }
        public bool CanAssign(string type)
        {
            return LockedType == null || LockedType == type;
        }
        public int TotalPlantedSlot()
        {
            return Entities.Count;
        }
        public bool TryAddOneEntity(string type, Func<FarmEntity> factory)
        {
            if (!CanAssign(type)) return false;
            if (IsFull) return false;
            Entities.Add(factory());
            return true;
        }

        public int HarvestAll(DateTime now, List<EquipmentEntity> equipments, out int removedCount)
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
                int baseAmount = entity.GetAvailableYield(now);
                int bonusAmount = (int)Math.Floor(baseAmount * totalMultiplier);

                // Land buff
                if (Land != null && Land.Config.YieldBuffPercent > 0)
                {
                    bonusAmount = (int)(bonusAmount * (1 + Land.Config.YieldBuffPercent / 100f));
                }

                entity.Harvest(now);
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

        public bool CanHarvestAny(DateTime now)
        {
            return Entities.Any(e => e.CanHarvest(now));
        }
        // Gộp logic hiển thị countdown chính
        public string GetRemainingTimeText(DateTime now)
        {
            // 1. Cleanup: Remove expired entities (quá 1h sau khi đầy trái)
            int removed = Entities.RemoveAll(e => e.IsExpired(now) && e.CheckDestroyItem(now));
            if (removed > 0)
            {
                OnEntitiesChanged?.Invoke(); // Gọi UI update lại
            }

            // 2. Ưu tiên: Cảnh báo cây đang sắp héo
            var rotEntity = Entities
                .Where(e => e.IsExpired(now))
                .OrderBy(e => e.GetSecondsUntilRot(now))
                .FirstOrDefault();

            if (rotEntity != null)
            {
                int rotSeconds = rotEntity.GetSecondsUntilRot(now);
                var rotSpan = TimeSpan.FromSeconds(rotSeconds);
                return $"Sắp héo! {(int)rotSpan.TotalHours:00}:{rotSpan.Minutes:00}:{rotSpan.Seconds:00}";
            }

            // 3. Countdown bình thường: Cây gần hết vòng đời nhất
            var nextEntity = Entities
                .Where(e => !e.IsExpired(now))
                .OrderBy(e => e.RemainingLifeSeconds(now))
                .FirstOrDefault();

            if (nextEntity != null)
            {
                int remaining = nextEntity.RemainingLifeSeconds(now);
                var remainSpan = TimeSpan.FromSeconds(remaining);
                return $"Còn {(int)remainSpan.TotalHours:00}:{remainSpan.Minutes:00}:{remainSpan.Seconds:00}";
            }

            return ""; // Không có cây nào để tính
        }

        public int GetTotalYielded() => Entities.Sum(e => e.Yielded);
        public int GetTotalMaxYield() => Entities.Sum(e => e.MaxYield);

        public bool IsDead(DateTime now)
        {
            return Entities.All(e => e.IsExpired(now));
        }
    }
}