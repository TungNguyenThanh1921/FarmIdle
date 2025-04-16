using System;

namespace CoreGamePlay
{
    public abstract class EquipmentEntity
    {
        public int Level { get; protected set; } = 1;

        public virtual void Upgrade()
        {
            Level++;
        }

        public abstract double GetYieldMultiplier();
    }
}