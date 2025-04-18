using Data;

namespace CoreGamePlay
{
    public class LandEntity
    {
        public LandConfigData Config { get; private set; }
        public int SlotCount => Config.SlotCount;
        public float YieldBuffPercent => Config.YieldBuffPercent;

        public LandEntity(LandConfigData config)
        {
            Config = config;
        }
    }
}