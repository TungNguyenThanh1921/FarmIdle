using System;

namespace CoreGamePlay
{
    public class Animal : FarmEntity
    {
        public Animal(string name, int intervalSeconds, int maxYield, DateTime bornAt) : base(name, intervalSeconds, maxYield, bornAt)
        {
        }
    }
}