using System;
using Newtonsoft.Json;

namespace CoreGamePlay
{
    public abstract class FarmEntity
    {
        [JsonProperty] public string Name { get; protected set; }
        [JsonProperty] public int YieldIntervalSeconds { get; protected set; }
        [JsonProperty] public int MaxYield { get; protected set; }
        [JsonProperty] public int Yielded { get; protected set; }
        [JsonProperty] public DateTime BornAt { get; protected set; }
        public FarmEntity(string name, int intervalSeconds, int maxYield, DateTime bornAt)
        {
            Name = name;
            YieldIntervalSeconds = intervalSeconds;
            MaxYield = maxYield;
            BornAt = bornAt;
            Yielded = 0;
        }

        public bool IsExpired(DateTime now)
        {
            int lifeTime = MaxYield * YieldIntervalSeconds;
            return (now - BornAt).TotalSeconds >= lifeTime;
        }

        public bool CanHarvest(DateTime now)
        {
            return GetAvailableYield(now) > 0;
        }

        public int GetAvailableYield(DateTime now)
        {
            int total = (int)((now - BornAt).TotalSeconds / YieldIntervalSeconds);
            int available = total - Yielded;
            return Math.Max(0, available);
        }

        public int Harvest(DateTime now, EquipmentEntity equipment = null)
        {
            int baseAmount = GetAvailableYield(now);
            int finalAmount = equipment != null ? equipment.ApplyBonus(baseAmount) : baseAmount;

            Yielded += baseAmount;
            return finalAmount;
        }
        public bool IsWaitingForFinalHarvest(DateTime now)
        {
            return Yielded >= MaxYield && !IsExpired(now);
        }
        // 1. Bao lâu nữa sẽ ra sản phẩm tiếp theo
        public int GetSecondsUntilNextYield(DateTime now)
        {
            int total = (int)((now - BornAt).TotalSeconds);
            int nextYieldAt = (Yielded + 1) * YieldIntervalSeconds;
            return Math.Max(0, nextYieldAt - total);
        }
        public int RemainingLifeSeconds(DateTime now)
        {
            int total = MaxYield * YieldIntervalSeconds;
            int elapsed = (int)(now - BornAt).TotalSeconds;
            return Math.Max(0, total - elapsed);
        }
        // 3. Sau khi đủ số sản phẩm, bao lâu sẽ phân hủy
        public int GetSecondsUntilRot(DateTime now)
        {
            if (!IsExpired(now)) return 0;
            var rotTime = BornAt
                .AddSeconds(MaxYield * YieldIntervalSeconds)
                .AddHours(1);
            return (int)Math.Max(0, (rotTime - now).TotalSeconds);
        }
        public bool CheckDestroyItem(DateTime now)
        {
            if (!IsExpired(now)) return false;
            var rotTime = BornAt
                .AddSeconds(MaxYield * YieldIntervalSeconds)
                .AddHours(1);
            int second = (int)Math.Max(0, (rotTime - now).TotalSeconds);
            if (second <= 0) return true;
            return false;
        }
    }
}