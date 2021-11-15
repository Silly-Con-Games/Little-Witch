using Assets.Scripts.GameEvents;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Ionic.Zip;
using System;

namespace Assets.Scripts.Analytics
{
    public class DataCollector : MonoBehaviour
    {
        static TimedEventHandler<MeleeAbilityEvent, MeleeData> meleeEventHandler = new TimedEventHandler<MeleeAbilityEvent, MeleeData>();

        public static readonly string dateFormat = "yyyy-MM-ddTHH-mm-ss.f";
        static DateTime from = DateTime.UtcNow;
        static DateTime to = DateTime.UtcNow;

        private void Awake()
        {
            GameEventQueue.AddListener(meleeEventHandler.GetEventType(), meleeEventHandler.HandleEvent);
            GameController.onGameStateChanged.AddListener(FlushOnStateChanged);
            Debug.Log(zipname);
            persistantDataPath = Application.persistentDataPath;
        }


        private void OnDestroy()
        {
            GameEventQueue.RemoveListener(meleeEventHandler.GetEventType(), meleeEventHandler.HandleEvent);
        }


        private void FlushOnStateChanged(EGameState s)
        {
            if (s == EGameState.GameOver)
                FlushToFiles();
            else if (s == EGameState.GameWon)
                FlushToFiles();
        }

        static string persistantDataPath = "";
        static string zipPath = persistantDataPath + Path.DirectorySeparatorChar + "ZippedFiles" + Path.DirectorySeparatorChar;

        static string analyticsPath = persistantDataPath + Path.DirectorySeparatorChar + "Analytics" + Path.DirectorySeparatorChar;
        static string zipname => $"{PlayerPrefs.GetString("player_name", "default")}_{from.ToString(dateFormat)}_{to.ToString(dateFormat)}.zip";

        static string zipDest => zipPath + zipname;

        public static void Finalize()
        {
            FlushToFiles();
            ZipFolder();
            UploadZippedFile();
        }

        public static void FlushToFiles()
        {
            if (!Directory.Exists(analyticsPath))
                Directory.CreateDirectory(analyticsPath);

            meleeEventHandler.WriteToDisk(analyticsPath);
        }

        public static void ZipFolder()
        {
            if (!Directory.Exists(zipPath))
                Directory.CreateDirectory(zipPath);

            using (ZipFile zip = new ZipFile())
            {                
                zip.AddDirectory(analyticsPath);
                // add the report into a different directory in the archive
                zip.Save(zipDest);

                Debug.Log($"Zipped directory {analyticsPath} to {zipDest}");
            }
        }

        public static void UploadZippedFile()
        {
            SimpleHttpClient.UploadFile(zipDest, zipname, "application/zip");
        }
    }
}