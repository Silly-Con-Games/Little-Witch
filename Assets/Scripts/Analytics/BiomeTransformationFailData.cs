using Assets.Scripts.GameEvents;
using System;

namespace Assets.Scripts.Analytics
{
    [Serializable]
    public struct BiomeTransformationFailData : ITimedData<BiomeTransformationFailedEvent>
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
            throw new System.NotImplementedException();
        }

        public int failCastCnt;
        public int noCdCnt;
        public int invalidTileCnt;
        public int reviveFailCnt;
        //Process the event here
        public void ProcessEvent(BiomeTransformationFailedEvent e)
        {
            failCastCnt++;
            noCdCnt += e.noEnergy ? 1 : 0;
            invalidTileCnt += e.invalidTile ? 1 : 0;
            reviveFailCnt += e.revive ? 1 : 0;
        }
    }
}
