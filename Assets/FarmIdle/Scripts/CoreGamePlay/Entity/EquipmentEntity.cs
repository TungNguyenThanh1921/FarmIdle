using System;
using Data;

namespace CoreGamePlay
{
    public class EquipmentEntity
    {
        public EquipmentConfigData Config { get; }
        public EquipmentEntity()
        {
            Config = EquipmentConfigLoader.GetDefault(); // fallback config
        }
        public EquipmentEntity(EquipmentConfigData config)
        {
            Config = config;
        }

        public double GetYieldMultiplier()
        {
            return 1 + (Config.BoostPercent / 100.0);
        }

        public int ApplyBonus(int baseAmount)
        {
            return (int)Math.Floor(baseAmount * GetYieldMultiplier());
        }
        public void Upgrade() => Config.Level++;
        // Tiện ích
        public string Id => Config.Id;
        public string DisplayName => Config.DisplayName;
        public int Price => Config.Price;
        public int Level => Config.Level;
    }
}