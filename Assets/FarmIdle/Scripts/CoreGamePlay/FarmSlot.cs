namespace CoreGamePlay
{
    public class FarmSlot
    {
        public FarmEntity Entity { get; private set; }
        public bool IsEmpty => Entity == null;

        public void AssignEntity(FarmEntity entity)
        {
            Entity = entity;
        }

        public void Clear() => Entity = null;
    }
}