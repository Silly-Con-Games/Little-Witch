using Assets.Scripts.GameEvents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Analytics
{
    public class TimedEventHandler<TEvent,TData> : IGameEventHandler
        where TEvent : TimedEvent
        where TData : struct, ITimedData<TEvent>        
    {        
        
        // Colection of the melee ability in totat
        TData total;
        // Snapshots of data in time, to see if player changes/improves
        [SerializeField]
        List<TData> snapshots = new List<TData>();
        TData currentSnapshot;

        const float snapshotInterval = 5; //in sec

        public TimedEventHandler()
        {
            total.Init(-1);
            currentSnapshot.Init(-1);
        }

        public Type GetEventType()
        {
            return typeof(TEvent);
        }

        public void HandleEvent(IGameEvent e)
        {
            TEvent ev = (TEvent)e;
            int snapshotIndex = (int)(ev.realTimeStart / snapshotInterval);
            int currentSnapshotIndex = currentSnapshot.GetSnapshotIndex();
            if (snapshotIndex != currentSnapshotIndex)
            {
                if(currentSnapshotIndex >= 0)
                    snapshots.Add(currentSnapshot);
                currentSnapshot = new TData();
                currentSnapshot.Init(snapshotIndex);
            }
            currentSnapshot.ProcessEvent(ev);
            total.ProcessEvent(ev);
        }

        public void WriteToFile(string path)
        {
            snapshots.Add(currentSnapshot);
            string serialized = "";

            for (int i = 0; i < snapshots.Count; i++)
            {
                serialized += JsonUtility.ToJson(snapshots[i], true) + ",\n";
            }

            path += typeof(TData).Name + ".json";
            Debug.Log($"Writing data to file {path}");

            if (!File.Exists(path))
            {
                File.Create(path);
                File.WriteAllText(path, serialized);
            }
            else
            {
                File.AppendAllText(path, serialized);
            }
                

            
             
        }
    }
}
