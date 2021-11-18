using Assets.Scripts.GameEvents;
using System;

namespace Assets.Scripts.Analytics
{
    [Serializable]
    public struct ForestAbilityData : ITimedData<ForestAbilityEvent>
    {
        // Used by timed event handler
        public int snapShotIndex;
        public int GetSnapshotIndex() => snapShotIndex;

        // Initialize your values here if needed
        public void Init(int index)
        {
            snapShotIndex = index;
            notprocessed = true;
        }

        public int successCastsCnt;
        public float damage;
        public float damagePath;
        public float damageEnd;
        public float rootDuration;
        public float enemiesRootedCnt;
        private bool notprocessed;
        // Check if this data struct received ANY events
        public bool NoEventsProcessed()
        {
            return notprocessed;
        }

        //Process the event here
        public void ProcessEvent(ForestAbilityEvent e)
        {
            notprocessed = false;
            successCastsCnt += e.cast ? 1 : 0;
            damage += e.damageEnd;
            damage += e.damagePath;
            damageEnd += e.damageEnd;
            damagePath += e.damagePath;
            rootDuration += e.rootDuration;
            enemiesRootedCnt += e.rootDuration > 0 ? 1 : 0;
        }
    }
}
