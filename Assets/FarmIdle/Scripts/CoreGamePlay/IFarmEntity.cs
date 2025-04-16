using System;

namespace CoreGamePlay
{
    public interface IFarmEntity
    {
        string Name { get; }
        bool CanHarvest(DateTime now);
        int Harvest(DateTime now);
        bool IsDead(DateTime now, TimeSpan gracePeriod);
        int GetAvailableYield(DateTime now);
    }
}