using System;

namespace CoreGamePlay
{
    public abstract class WorkerEntity
    {
        public bool IsBusy { get; protected set; }
        public DateTime TaskStartedAt { get; protected set; }
        public TimeSpan TaskDuration { get; protected set; }

        public virtual void AssignTask(TimeSpan duration, DateTime startTime)
        {
            IsBusy = true;
            TaskStartedAt = startTime;
            TaskDuration = duration;
        }

        public bool IsTaskDone(DateTime now) => IsBusy && now >= TaskStartedAt + TaskDuration;
        public void CompleteTask() => IsBusy = false;

        public abstract string GetRole(); // Ex: "Generalist", "CowWorker", "PlantWorker"
    }
}