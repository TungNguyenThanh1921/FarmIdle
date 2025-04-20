using System;
using CoreBase;
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
        public void AddYield(int amount)
        {
            Yielded += amount;
            if (Yielded > MaxYield)
                Yielded = MaxYield;
        }
        public bool IsExpired(ITimeProvider time)
        {
            var now = time.Now;
            int lifeTime = MaxYield * YieldIntervalSeconds;
            return (now - BornAt).TotalSeconds >= lifeTime;
        }

        public bool CanHarvest(ITimeProvider time) => GetAvailableYield(time) > 0;

        public int GetAvailableYield(ITimeProvider time)
        {
            int total = (int)((time.Now - BornAt).TotalSeconds / YieldIntervalSeconds);
            int available = total - Yielded;
            return Math.Max(0, available);
        }

        public int Harvest(ITimeProvider time, EquipmentEntity equipment = null)
        {
            int baseAmount = GetAvailableYield(time);
            if (baseAmount <= 0) return 0;
            int finalAmount = equipment != null ? equipment.ApplyBonus(baseAmount) : baseAmount;
            Yielded += baseAmount;
            return finalAmount;
        }
        public int GetSecondsUntilNextYield(ITimeProvider time)
        {
            int total = (int)((time.Now - BornAt).TotalSeconds);
            int nextYieldAt = (Yielded + 1) * YieldIntervalSeconds;
            return Math.Max(0, nextYieldAt - total);
        }

        public int RemainingLifeSeconds(ITimeProvider time)
        {
            int total = MaxYield * YieldIntervalSeconds;
            int elapsed = (int)(time.Now - BornAt).TotalSeconds;
            return Math.Max(0, total - elapsed);
        }

        public int GetSecondsUntilRot(ITimeProvider time)
        {
            if (!IsExpired(time)) return 0;
            var rotTime = BornAt.AddSeconds(MaxYield * YieldIntervalSeconds).AddHours(1);
            return (int)Math.Max(0, (rotTime - time.Now).TotalSeconds);
        }

        public bool CheckDestroyItem(ITimeProvider time)
        {
            if (!IsExpired(time)) return false;
            return GetSecondsUntilRot(time) <= 0;
        }
    }
}