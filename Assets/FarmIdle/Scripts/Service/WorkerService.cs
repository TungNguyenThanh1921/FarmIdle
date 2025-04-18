using System;
using System.Collections.Generic;
using CoreGamePlay;
using Data;

namespace Service
{
    public class WorkerService
    {
        private readonly UserData _userData;

        public WorkerService(UserData userData)
        {
            _userData = userData;
        }

        public int TotalWorkers => _userData.Workers.Count;

        public int BusyWorkerCount(DateTime now)
        {
            int count = 0;
            foreach (var worker in _userData.Workers)
            {
                if (worker.IsBusy && !worker.IsTaskDone(now))
                    count++;
            }
            return count;
        }

        public int IdleWorkerCount(DateTime now)
        {
            return TotalWorkers - BusyWorkerCount(now);
        }

        public bool TryAssignWorkerToTask(TimeSpan duration, DateTime now, Action taskCallback = null)
        {
            foreach (var worker in _userData.Workers)
            {
                if (!worker.IsBusy)
                {
                    worker.AssignTask(now);
                    taskCallback?.Invoke();
                    return true;
                }
            }
            return false;
        }

        public void CompleteFinishedTasks(DateTime now)
        {
            foreach (var worker in _userData.Workers)
            {
                if (worker.IsBusy && worker.IsTaskDone(now))
                {
                    worker.CompleteTask();
                }
            }
        }

        public void HireWorker()
        {
            _userData.Workers.Add(new WorkerEntity());
        }
    }
}