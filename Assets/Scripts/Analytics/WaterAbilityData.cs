using Assets.Scripts.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Analytics
{
    public struct WaterAbilityData : ITimedData<WaterAbilityEvent>
    {
        public int snapShotIndex;
        public int successCastsCnt;
        public int enemiesPushedCnt;
        public int projectilesDestroyedCnt;

        public int GetSnapshotIndex()
        {
            return snapShotIndex;
        }

        public void Init(int index)
        {
            this.snapShotIndex = index;
        }

        public bool NoEventsProcessed()
        {
            return enemiesPushedCnt == 0 && projectilesDestroyedCnt == 0;
        }

        public void ProcessEvent(WaterAbilityEvent e)
        {
            enemiesPushedCnt += e.pushedEnemy ? 1 : 0;
            projectilesDestroyedCnt += e.killedProjectile? 1 : 0;
            successCastsCnt += e.cast ? 1 : 0;
        }
    }
}
