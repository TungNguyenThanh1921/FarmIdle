using System;

namespace CoreGamePlay
{
    public class Crop : FarmEntity
    {
        public Crop(string name, int intervalSeconds, int maxYield, DateTime bornAt) : base(name, intervalSeconds, maxYield, bornAt)
        {
        }
    }
}