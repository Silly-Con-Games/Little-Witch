using Assets.Scripts.GameEvents;
using System;

namespace Assets.Scripts.Analytics
{
    [Serializable]
    public struct MeadowAbilityData : ITimedData<MeadowAbilityEvent>
    {
        // Used by timed event handler
        public int snapShotIndex;
        public int GetSnapshotIndex() => snapShotIndex;

        // Initialize your values here if needed
        public void Init(int index)
        {
            snapShotIndex = index;
        }

        public float damage;
        public int enemiesHitCnt;
        public int successCastsCnt;
        // Check if this data struct received ANY events
        public bool NoEventsProcessed()
        {
            return successCastsCnt == 0 && enemiesHitCnt == 0;
        }
        
        //Process the event here
        public void ProcessEvent(MeadowAbilityEvent e)
        {
            damage += e.damage;
            enemiesHitCnt++;
            successCastsCnt += e.cast ? 1 : 0;
        }
    }
}
