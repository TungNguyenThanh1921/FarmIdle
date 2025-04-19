using System;
using System.Collections.Generic;
using CoreBase;
using Data;

namespace Service
{
    public class OfflineProgressService
    {
        private readonly UserData _userData;

        private readonly ITimeProvider time;
        public OfflineProgressService(UserData userData, ITimeProvider timeProvider)
        {
            _userData = userData;
            time = timeProvider;
        }

        public TimeSpan GetOfflineDuration()
        {
            return time.Now - _userData.LastExitTime.Now;
        }

        public List<OfflineYieldResult> CalculateOfflineYield()
        {
            var results = new List<OfflineYieldResult>();
            var now = time.Now;

            foreach (var slot in _userData.Slots)
            {
                if (slot.IsEmpty) continue;

                foreach (var entity in slot.Entities)
                {
                    int available = entity.GetAvailableYield(time);
                    if (available > 0)
                    {
                        results.Add(new OfflineYieldResult
                        {
                            Name = entity.Name,
                            SlotIndex = _userData.Slots.IndexOf(slot),
                            Amount = available
                        });

                        entity.Harvest(time);
                    }
                }
            }

            return results;
        }

        public void SaveCurrentTimeAsExit()
        {
            _userData.SaveExitTime(time);
        }
    }

    public class OfflineYieldResult
    {
        public string Name;
        public int SlotIndex;
        public int Amount;
    }
}