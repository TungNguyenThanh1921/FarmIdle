using System;

namespace CoreGamePlay
{
    public class Crop : FarmEntity
    {
        public Crop(string name, TimeSpan interval, int maxYield, DateTime plantedAt)
           : base(name, interval, maxYield, plantedAt)
        {
        }

        public override int Harvest(DateTime now)
        {
            int amount = GetAvailableYield(now);
            Yielded += amount;
            return amount;
        }
    }
}