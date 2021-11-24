using Assets.Scripts.GameEvents;
using System;

namespace Assets.Scripts.Analytics
{
    [Serializable]
    public struct BiomeTransformationData : ITimedData<BiomeTransformedEvent>
    {
        // Used by timed event handler
        public int snapShotIndex;
        public int GetSnapshotIndex() => snapShotIndex;

        // Initialize your values here if needed
        public void Init(int index)
        {
            snapShotIndex = index;
        }

        public int castCnt;
        public int diedToEnemyCnt;
        public int reviveCnt;
        public int forestNewCnt;
        public int waterNewCnt;
        public int meadowNewCnt;
        public int deadTransformedCnt;
        // Check if this data struct received ANY events
        public bool NoEventsProcessed()
        {
            return castCnt == 0 && reviveCnt == 0 && diedToEnemyCnt == 0;
        }

        //Process the event here
        public void ProcessEvent(BiomeTransformedEvent e)
        {
            castCnt += e.playerOrigin ? 1 : 0;
            reviveCnt += e.revive ? 1 : 0;
            diedToEnemyCnt += e.enemyOrigin ? 1 : 0;

            if(! e.revive && e.playerOrigin)
            {
                forestNewCnt += e.to == BiomeType.FOREST ? 1 : 0;
                meadowNewCnt += e.to == BiomeType.MEADOW ? 1 : 0;
                waterNewCnt += e.to == BiomeType.WATER ? 1 : 0;
                deadTransformedCnt += e.from == BiomeType.DEAD ? 1 : 0;
            }
        }
    }
}
