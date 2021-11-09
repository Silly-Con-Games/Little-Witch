using Assets.Scripts.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Analytics
{
    public class TimedEventHandler<TEvent,TData> : IGameEventHandler
        where TEvent : TimedEvent
        where TData : struct, ITimedData<TEvent>        
    {        
        
        // Colection of the melee ability in totat
        TData total;
        // Snapshots of data in time, to see if player changes/improves
        List<TData> snapshots = new List<TData>();
        TData currentSnapshot;

        const float snapshotInterval = 5; //in sec

        public TimedEventHandler()
        {
            total.Init(-1);          
        }

        public Type GetEventType()
        {
            return typeof(TData);
        }

        public void HandleEvent(IGameEvent e)
        {
            TEvent ev = (TEvent)e;
            int snapshotIndex = (int)(ev.realTimeStart / snapshotInterval);

            if (snapshotIndex != currentSnapshot.GetSnapshotIndex())
            {
                snapshots.Add(currentSnapshot);
                currentSnapshot = new TData();
                currentSnapshot.Init(snapshotIndex);
            }
            currentSnapshot.ProcessEvent(ev);
            total.ProcessEvent(ev);
        }
    }
}
