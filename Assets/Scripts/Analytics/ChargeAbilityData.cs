using Assets.Scripts.GameEvents;
using System;

namespace Assets.Scripts.Analytics
{
    [Serializable]
    public struct ChargeAbilityData : ITimedData<ChargeAbilityEvent>
    {
        // Used by timed event handler
        public int snapShotIndex;
        public int GetSnapshotIndex() => snapShotIndex;

        // Initialize your values here if needed
        public void Init(int index)
        {
            snapShotIndex = index;
        }

        // Check if this data struct received ANY events
        public bool NoEventsProcessed()
        {
            return failedCastsCnt == 0 && successCastsCnt == 0;
        }

        public int failedCastsCnt;
        public int successCastsCnt;
        public float damage;
        public float energyCost;

        //Process the event here
        public void ProcessEvent(ChargeAbilityEvent e)
        {
            failedCastsCnt += e.failCast ? 1 : 0;
            successCastsCnt += e.cast ? 1 : 0;
            energyCost += e.energyCost;
            damage += e.damage;
        }
    }
}
