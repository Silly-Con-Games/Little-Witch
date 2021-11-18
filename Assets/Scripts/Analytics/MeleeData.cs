using Assets.Scripts.GameEvents;
using System;

namespace Assets.Scripts.Analytics
{
    [Serializable]
    public struct MeleeData : ITimedData<MeleeAbilityEvent>
    {
        public int snapShotIndex;
        public float damageDealt;
        public int failedCastsCnt;
        public int successCastsCnt;

        public void ProcessEvent(MeleeAbilityEvent e)
        {
            damageDealt += e.damageDealt;
            failedCastsCnt += e.failedCast ? 1 : 0;
            successCastsCnt += e.cast ? 1 : 0;
        }

        public void Init(int index)
        {
            snapShotIndex = index;
            damageDealt = 0;
            failedCastsCnt = 0;
            successCastsCnt = 0;
        }

        public int GetSnapshotIndex() => snapShotIndex;

        public bool NoEventsProcessed()
        {
            return damageDealt == 0 && failedCastsCnt == 0 && successCastsCnt == 0;
        }
    }
}
