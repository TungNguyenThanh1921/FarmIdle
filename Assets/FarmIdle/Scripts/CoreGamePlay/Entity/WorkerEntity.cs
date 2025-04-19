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

        public WorkerEntity() => Config = WorkerConfigLoader.GetDefault();

        public WorkerEntity(WorkerConfigData config) => Config = config;
        [JsonProperty] public int? WorkingSlotIndex { get; private set; } = null;
        public void AssignTask(ITimeProvider time, int slotIndex)
        {
            IsBusy = true;
            TaskStartedAt = time.Now;
            WorkingSlotIndex = slotIndex;
        }

        public void CancelTask()
        {
            IsBusy = false;
            WorkingSlotIndex = null;
        }

        public bool IsTaskDone(ITimeProvider time)
        {
            return IsBusy && time.Now >= TaskStartedAt + TaskDuration;
        }

        public void CompleteTask()
        {
            IsBusy = false;
            WorkingSlotIndex = null;
        }
        public bool IsIdle(ITimeProvider time) => !IsBusy || IsTaskDone(time);

        public string Id => Config.Id;
        public string DisplayName => Config.DisplayName;
        public bool IsAuto => Config.AutoHarvest;
        public string Role => Config.Role;
        public int Price => Config.Price;
    }
}