using System;
namespace CoreGamePlay
{
    public class FarmEquipment : EquipmentEntity
    {
        private const double BonusPerLevel = 0.10;

        public override double GetYieldMultiplier()
        {
            return 1 + (Level - 1) * BonusPerLevel;
        }

        public int ApplyBonus(int baseYield)
        {
            return (int)Math.Ceiling(baseYield * GetYieldMultiplier());
        }
    }

}
