using Assets.Scripts.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Analytics
{
    public class MeleeData : BaseData
    {        
        struct Data 
        {
            public float snapShotIndex;
            public float damageDealt;
            public int failedCastsCnt;
            public int successCastsCnt;

            public void ProcessEvent(MeleeAbilityEvent e)
            {
                damageDealt += e.damageDealt;
                failedCastsCnt += e.failedCast ? 1 : 0;
                successCastsCnt += e.cast ? 1 : 0;
            }

            public Data(int snapShotIndex) 
            {
                this.snapShotIndex = snapShotIndex;
                damageDealt = 0;
                failedCastsCnt = 0;
                successCastsCnt = 0;
            }
        }

        // Colection of the melee ability in totat
        Data total;
        // Snapshots of data in time, to see if player changes/improves
        List<Data> snapshots = new List<Data>();
        Data currentSnapshot;

        const float snapshotInterval = 5; //in sec

        public MeleeData()
        {
            total = new Data(-1);            
        }

        public override Type GetEventType()
        {
            return typeof(MeleeAbilityEvent);
        }

        public override void HandleEvent(IGameEvent e)
        {
            MeleeAbilityEvent ev = (MeleeAbilityEvent)e;
            int snapshotIndex = (int)(ev.realTimeStart / snapshotInterval);
            
            if(snapshotIndex != currentSnapshot.snapShotIndex)
            {
                snapshots.Add(currentSnapshot);
                currentSnapshot = new Data(snapshotIndex);
            }
            currentSnapshot.ProcessEvent(ev);
            total.ProcessEvent(ev);
        }

    }
}
