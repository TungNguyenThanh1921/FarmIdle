using System;

namespace CoreGamePlay
{
    public abstract class FarmEntity
    {
        public string Name { get; protected set; }
        public TimeSpan YieldInterval { get; protected set; }
        public int MaxYield { get; protected set; }
        public int Yielded { get; protected set; }
        public DateTime StartTime { get; protected set; }

        public FarmEntity(string name, TimeSpan interval, int maxYield, DateTime startTime)
        {
            Name = name;
            YieldInterval = interval;
            MaxYield = maxYield;
            StartTime = startTime;
            Yielded = 0;
        }

        public virtual int GetAvailableYield(DateTime now)
        {
            int total = (int)((now - StartTime).TotalMinutes / YieldInterval.TotalMinutes);
            total = Math.Min(total, MaxYield);
            return Math.Max(0, total - Yielded);
        }

        public virtual bool CanHarvest(DateTime now) => GetAvailableYield(now) > 0;

        public virtual bool IsDead(DateTime now, TimeSpan gracePeriod)
        {
            var end = StartTime + YieldInterval * MaxYield;
            return now > end + gracePeriod;
        }

        public abstract int Harvest(DateTime now);
    }
}