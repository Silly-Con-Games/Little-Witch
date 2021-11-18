using Assets.Scripts.GameEvents;
using System;

namespace Assets.Scripts.Analytics
{
    [Serializable]
    public struct MainAbilityFailData : ITimedData<MainAbilityFailEvent>
    {
        // Used by timed event handler
        public int snapShotIndex;

        public int notOnCdCnt;
        public int deadBiomeCnt;
        public int failedCastCnt;

        public int GetSnapshotIndex() => snapShotIndex;

        // Initialize your values here if needed
        public void Init(int index)
        {
            snapShotIndex = index;
        }

        // Check if this data struct received ANY events
        public bool NoEventsProcessed()
        {
            return failedCastCnt == 0;
        }

        //Process the event here
        public void ProcessEvent(MainAbilityFailEvent e)
        {
            deadBiomeCnt += e.deadBiome ? 1 : 0;
            notOnCdCnt += e.notOnCd ? 1 : 0;
            failedCastCnt++;
        }
    }
}
