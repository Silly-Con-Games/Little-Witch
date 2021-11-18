using Assets.Scripts.GameEvents;
using System;

namespace Assets.Scripts.Analytics
{
    [Serializable]
    public struct DashData : ITimedData<DashAbilityEvent>
    {
        // Used by timed event handler
        public int snapShotIndex;
        public int GetSnapshotIndex() => snapShotIndex;

        // Initialize your values here if needed
        public void Init(int index)
        {
            snapShotIndex = index;
        }

        public float distance;
        public float successCastsCnt;
        // Check if this data struct received ANY events
        public bool NoEventsProcessed()
        {
            return distance == 0;
        }

        //Process the event here
        public void ProcessEvent(DashAbilityEvent e)
        {
            distance += e.distance;
            successCastsCnt++;
        }
    }
}
