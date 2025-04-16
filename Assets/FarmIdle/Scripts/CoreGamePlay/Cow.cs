using System;

namespace CoreGamePlay
{
    public class Cow : FarmEntity
    {
        public Cow(string name, TimeSpan interval, int maxYield, DateTime bornAt)
            : base(name, interval, maxYield, bornAt)
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