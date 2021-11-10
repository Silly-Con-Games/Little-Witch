using Assets.Scripts.GameEvents;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Analytics
{
    public class DataCollector : MonoBehaviour
    {
        TimedEventHandler<MeleeAbilityEvent, MeleeData> meleeEventHandler = new TimedEventHandler<MeleeAbilityEvent, MeleeData>();
        // Start is called before the first frame update
        void Start()
        {
            GameEventQueue.AddListener(meleeEventHandler.GetEventType(), meleeEventHandler.HandleEvent);
        }

        public void FlushToFiles()
        {
            string path = Application.streamingAssetsPath +
                Path.DirectorySeparatorChar +
                "Test" +
                Path.DirectorySeparatorChar +
                "test.txt";
            meleeEventHandler.WriteToFile(path);
        }

        private void OnDestroy()
        {
            FlushToFiles();
            GameEventQueue.RemoveListener(meleeEventHandler.GetEventType(), meleeEventHandler.HandleEvent);
        }
    }
}