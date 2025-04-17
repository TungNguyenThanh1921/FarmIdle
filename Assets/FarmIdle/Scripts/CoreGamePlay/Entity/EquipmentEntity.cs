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

        /// <summary>
        /// Hệ số bonus sản lượng, mỗi cấp tăng 10%
        /// </summary>
        public virtual double GetYieldMultiplier()
        {
            return 1 + (Level - 1) * 0.1; // Lv1 = 1.0, Lv2 = 1.1, ...
        }

        /// <summary>
        /// Tính toán sản lượng đã tăng theo cấp thiết bị
        /// </summary>
        public virtual int ApplyBonus(int baseAmount)
        {
            double bonus = GetYieldMultiplier();
            return (int)Math.Floor(baseAmount * bonus);
        }
    }
}