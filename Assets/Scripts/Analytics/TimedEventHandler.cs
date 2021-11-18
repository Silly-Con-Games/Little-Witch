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


        float snapshotInterval = 5; //in sec

        public TimedEventHandler()
        {
            Reset();
        }

        public TimedEventHandler(float interval) : this()
        {
            snapshotInterval = interval;
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

        DateTime from;
        DateTime to;
        string fileName => $"{from.ToString(DataCollector.dateFormat)}_{to.ToString(DataCollector.dateFormat)}.json";
        string totalFileName => $"total_{fileName}";
        public string directory => typeof(TData).Name;

        public void WriteToDisk(string path)
        {
            if (total.NoEventsProcessed())
                return;

            snapshots.Add(currentSnapshot);
            to = DateTime.UtcNow;

            path += directory + Path.DirectorySeparatorChar;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string filePath = path + fileName;
            string totalFilePath = path + totalFileName;


            Debug.Log($"Writing data to file {filePath}");

            if (!File.Exists(filePath))
            {
                string serialized = "";

                for (int i = 0; i < snapshots.Count; i++)
                {
                    serialized += JsonUtility.ToJson(snapshots[i], true) + ",\n";
                }
                File.WriteAllText(filePath, serialized);
            }
            else
                Debug.LogError("File alreadt exists!, aborting write");

            Debug.Log($"Writing total to file {totalFilePath}");

            if (!File.Exists(totalFilePath))
            {                
                File.WriteAllText(totalFilePath, JsonUtility.ToJson(total, true));
            }
            else
                Debug.LogError("File already exists!, aborting write");

            Reset();            
        }

        void Reset()
        {
            total = new TData();
            currentSnapshot = new TData();
            snapshots = new List<TData>();
            total.Init(-1);
            currentSnapshot.Init(-1);
            to = from = DateTime.UtcNow;
        }
    }
}
