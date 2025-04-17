using System;

namespace CoreGamePlay
{
    public abstract class FarmEntity
    {
        public string Name { get; protected set; }
        public int YieldIntervalSeconds { get; protected set; }
        public int MaxYield { get; protected set; }
        public int Yielded { get; protected set; }
        public DateTime BornAt { get; protected set; }
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

        public int GetRemainingSeconds(DateTime now)
        {
            int totalSeconds = MaxYield * YieldIntervalSeconds;
            int elapsed = (int)(now - BornAt).TotalSeconds;
            return Math.Max(0, totalSeconds - elapsed);
        }
    }
}