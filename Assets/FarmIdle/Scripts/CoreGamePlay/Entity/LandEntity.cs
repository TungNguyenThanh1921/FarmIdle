using Data;

namespace CoreGamePlay
{
    public class LandEntity
    {
        public LandConfigData Config { get; private set; }

        public LandEntity(LandConfigData config)
        {
            Config = config;
        }
    }
}