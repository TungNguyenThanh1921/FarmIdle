using System;
using System.Collections.Generic;
using Data;

namespace Service
{
    public class OfflineProgressService
    {
        private readonly UserData _userData;

        public OfflineProgressService(UserData userData)
        {
            _userData = userData;
        }

        public TimeSpan GetOfflineDuration()
        {
            return DateTime.Now - _userData.LastExitTime;
        }

        public List<OfflineYieldResult> CalculateOfflineYield()
        {
            var results = new List<OfflineYieldResult>();
            var now = DateTime.Now;

            foreach (var slot in _userData.Slots)
            {
                if (slot.IsEmpty) continue;

                var entity = slot.Entity;
                int available = entity.GetAvailableYield(now);

                if (available > 0)
                {
                    results.Add(new OfflineYieldResult
                    {
                        Name = entity.Name,
                        SlotIndex = _userData.Slots.IndexOf(slot),
                        Amount = available
                    });

                    // Mark as already harvested (for logic consistency)
                    entity.Harvest(now);
                }
            }

            return results;
        }

        public void SaveCurrentTimeAsExit()
        {
            _userData.LastExitTime = DateTime.Now;
        }
    }

    public class OfflineYieldResult
    {
        public string Name;
        public int SlotIndex;
        public int Amount;
    }
}