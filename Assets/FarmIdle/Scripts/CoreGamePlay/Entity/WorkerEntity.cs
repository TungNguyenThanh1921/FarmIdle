using System;
using Data;

namespace CoreGamePlay
{
    public class WorkerEntity
    {
        public WorkerConfigData Config { get; }
        public bool IsBusy { get; private set; }
        public DateTime TaskStartedAt { get; private set; }
        public TimeSpan TaskDuration { get; private set; }
        public WorkerEntity()
        {
            Config = WorkerConfigLoader.GetDefault(); // fallback config
        }
        public WorkerEntity(WorkerConfigData config)
        {
            Config = config;
            TaskDuration = TimeSpan.FromSeconds(config.ActionTimeSeconds);
        }

        public void AssignTask(DateTime startTime)
        {
            IsBusy = true;
            TaskStartedAt = startTime;
        }

        public bool IsTaskDone(DateTime now) => IsBusy && now >= TaskStartedAt + TaskDuration;
        public void CompleteTask() => IsBusy = false;

        // Tiện ích
        public string Id => Config.Id;
        public string DisplayName => Config.DisplayName;
        public bool IsAuto => Config.AutoHarvest;
        public int Price => Config.Price;
    }
}