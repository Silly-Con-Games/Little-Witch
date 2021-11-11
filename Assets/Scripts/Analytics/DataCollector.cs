using Assets.Scripts.GameEvents;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Ionic.Zip;
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

        string zipPath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "ZippedFiles" + Path.DirectorySeparatorChar;
        string path = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Test" + Path.DirectorySeparatorChar;
        string zipname = "test.zip";

        string zipDest => zipPath + zipname;

        public void FlushToFiles()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            meleeEventHandler.WriteToFile(path);
        }

        public void ZipFolder()
        {
            if (!Directory.Exists(zipPath))
                Directory.CreateDirectory(zipPath);

            using (ZipFile zip = new ZipFile())
            {                
                zip.AddDirectory(path);
                // add the report into a different directory in the archive
                zip.Save(zipDest);

                Debug.Log($"Zipped directory {path} to {zipDest}");
            }
        }

        public void UploadZippedFile()
        {
            SimpleHttpClient.UploadFile(zipDest, zipname, "application/zip");
        }

        private void OnDestroy()
        {
            FlushToFiles();
            ZipFolder();
            GameEventQueue.RemoveListener(meleeEventHandler.GetEventType(), meleeEventHandler.HandleEvent);
        }
    }
}