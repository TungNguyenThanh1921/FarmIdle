using System;

namespace CoreGamePlay
{
   public class Worker
    {
        public bool IsBusy { get; private set; }
        public DateTime TaskStartedAt { get; private set; }
        public TimeSpan TaskDuration { get; private set; }

        public void AssignTask(TimeSpan duration, DateTime startTime)
        {
            IsBusy = true;
            TaskStartedAt = startTime;
            TaskDuration = duration;
        }

        public bool IsTaskDone(DateTime now) => IsBusy && now >= TaskStartedAt + TaskDuration;

        public void CompleteTask() => IsBusy = false;
    }
}