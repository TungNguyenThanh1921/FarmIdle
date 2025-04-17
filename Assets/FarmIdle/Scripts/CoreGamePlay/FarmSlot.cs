using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CoreGamePlay
{
    public class FarmSlot
    {
        [JsonProperty]
        public string LockedType { get; private set; }
        [JsonProperty]
        public List<FarmEntity> Entities { get; private set; } = new();

        private const int MaxEntityPerSlot = 5;

        public bool IsEmpty => Entities.Count == 0;
        public bool IsFull => Entities.Count >= MaxEntityPerSlot;
        // Callback để thông báo UI cần update lại
        public Action OnEntitiesChanged;

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

        public int HarvestAll(DateTime now, EquipmentEntity equipment, out int removedCount)
        {
            removedCount = 0;
            int totalYield = 0;

            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                var entity = Entities[i];
                int yield = entity.Harvest(now, equipment);
                totalYield += yield;

                if (entity.Yielded >= entity.MaxYield)
                {
                    Entities.RemoveAt(i); // cây chết
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
            // 1. Dọn dẹp: Xoá các cây đã héo (quá 1h sau khi full trái)
            int removed = Entities.RemoveAll(e => e.IsExpired(now) && e.CheckDestroyItem(now));
            if (removed > 0)
            {
                OnEntitiesChanged?.Invoke(); // thông báo UI cập nhật lại
            }

            // Ưu tiên hiển thị cây đã full trái và đang đếm ngược đến thời điểm héo
            var rotEntity = Entities
                .Where(e => e.IsExpired(now))
                .OrderBy(e => e.GetSecondsUntilRot(now))
                .FirstOrDefault();

            if (rotEntity != null)
            {
                int rotSeconds = rotEntity.GetSecondsUntilRot(now);
                var rotSpan = TimeSpan.FromSeconds(rotSeconds);
                return $"Sắp héo! {rotSpan:hh\\:mm\\:ss}";
            }
            else
            {
                var firstHarvestEntity = Entities
                    .Where(e => !e.IsExpired(now))
                    .OrderBy(e => e.RemainingLifeSeconds(now))
                    .FirstOrDefault();

                if (firstHarvestEntity != null)
                {
                    int remainSeconds = firstHarvestEntity.RemainingLifeSeconds(now);
                    var remainSpan = TimeSpan.FromSeconds(remainSeconds);
                    return $"Còn {remainSpan:hh\\:mm\\:ss}";
                }
                return "";
            }
        }

        public int GetTotalYielded() => Entities.Sum(e => e.Yielded);
        public int GetTotalMaxYield() => Entities.Sum(e => e.MaxYield);

        public bool IsDead(DateTime now)
        {
            return Entities.All(e => e.IsExpired(now));
        }
    }
}