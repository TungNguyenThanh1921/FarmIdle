using System;
using Data;
using Newtonsoft.Json;
using CoreBase;

namespace CoreGamePlay
{
    public class WorkerEntity
    {
        [JsonProperty] public WorkerConfigData Config { get; private set; }
        [JsonProperty] public bool IsBusy { get; private set; }
        [JsonProperty] public DateTime TaskStartedAt { get; private set; }
        [JsonIgnore] public TimeSpan TaskDuration => TimeSpan.FromSeconds(Config.ActionTimeSeconds);
        public WorkerEntity(WorkerConfigData config) => Config = config;
        [JsonProperty] public int? WorkingSlotIndex { get; private set; } = null;
        public void AssignTask(ITimeProvider time, int slotIndex)
        {
            IsBusy = true;
            TaskStartedAt = time.Now;
            WorkingSlotIndex = slotIndex;
        }

        public bool IsTaskDone(ITimeProvider time)
        {
            return IsBusy && time.Now >= TaskStartedAt + TaskDuration;
        }

        public void CompleteTask()
        {
            IsBusy = false;
        }
        public bool IsAssignedToSlot() => WorkingSlotIndex == null;
        public void AssignSlot(int slotIndex) => WorkingSlotIndex = slotIndex;
    }
}