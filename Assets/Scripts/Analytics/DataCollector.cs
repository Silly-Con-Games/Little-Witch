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
        static TimedEventHandler<MeleeAbilityEvent, MeleeData> meleeEventHandler = new TimedEventHandler<MeleeAbilityEvent, MeleeData>();

        void Start()
        {
            GameEventQueue.AddListener(meleeEventHandler.GetEventType(), meleeEventHandler.HandleEvent);
        }

        private void OnDestroy()
        {

            GameEventQueue.RemoveListener(meleeEventHandler.GetEventType(), meleeEventHandler.HandleEvent);
        }

        static string zipPath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "ZippedFiles" + Path.DirectorySeparatorChar;
        static string path = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Test" + Path.DirectorySeparatorChar;
        static string zipname = "test.zip";

        static string zipDest => zipPath + zipname;

        public static void Finalize()
        {
            FlushToFiles();
            ZipFolder();
            UploadZippedFile();
        }

        public static void FlushToFiles()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            meleeEventHandler.WriteToFile(path);
        }

        public static void ZipFolder()
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

        public static void UploadZippedFile()
        {
            SimpleHttpClient.UploadFile(zipDest, zipname, "application/zip");
        }
    }
}