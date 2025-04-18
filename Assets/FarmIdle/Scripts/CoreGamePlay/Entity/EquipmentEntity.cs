using System;
using Data;
using Newtonsoft.Json;

namespace CoreGamePlay
{
    public class EquipmentEntity
    {
        [JsonProperty]
        public EquipmentConfigData Config { get; private set; }
        [JsonProperty]
        public int Level { get; private set; }

        public EquipmentEntity(EquipmentConfigData config)
        {
            Config = config;
            Level = config.Level;
        }
        public double GetYieldMultiplier()
        {
            return 1 + Level * (Config.BoostPercent / 100f);
        }

        public int ApplyBonus(int baseAmount)
        {
            return (int)Math.Floor(baseAmount * GetYieldMultiplier());
        }

        public bool CanUpgrade()
        {
            return Level < Config.MaxLevel;
        }
        public double GetPercentBoost()
        {
            return Level * Config.BoostPercent;
        }
        public int GetUpgradeCost()
        {
            return (int)(Config.UpgradeCostBase * Math.Pow(1.5f, Level - 1)); // Tăng theo cấp
        }

        public void Upgrade()
        {
            if (CanUpgrade())
            {
                Level++;
            }
        }
    }
}