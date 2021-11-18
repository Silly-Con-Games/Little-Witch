using Assets.Scripts.GameEvents;
using System.IO;
using UnityEngine;
using Ionic.Zip;
using System;
using System.Text;

namespace Assets.Scripts.Analytics
{
    public static class DataCollector 
    {
        static float timeInterval = 5;
        static TimedEventHandler<MeleeAbilityEvent, MeleeData> meleeEventHandler = new TimedEventHandler<MeleeAbilityEvent, MeleeData>(timeInterval);

        public static readonly string dateFormat = "yyyy-MM-ddTHH-mm-ss.f";
        static DateTime from = DateTime.UtcNow;
        static DateTime to;

        static bool initialized = false;
        [RuntimeInitializeOnLoadMethod]
        static void RunOnStart()
        {
            if (!initialized)
            {
                persistantDataPath = Application.persistentDataPath;
                Application.quitting += OnApplicationQuit_Internal;
                CreateConfig();
                initialized = true;
                GameEventQueue.AddListener(meleeEventHandler.GetEventType(), meleeEventHandler.HandleEvent);
                GameController.onGameStateChanged.AddListener(FlushOnGameWonOrOver);
            }
            
        }

        private static void OnApplicationQuit_Internal()
        {
            GameEventQueue.RemoveListener(meleeEventHandler.GetEventType(), meleeEventHandler.HandleEvent);
            ZipAndSendData();
        }

        private static void FlushOnGameWonOrOver(EGameState s)
        {
            if (s == EGameState.GameOver || s == EGameState.GameWon)
                FlushToFiles();
        }

        static string persistantDataPath;
        static string zipPath => persistantDataPath + Path.DirectorySeparatorChar + "ZippedFiles" + Path.DirectorySeparatorChar;
        static string analyticsPath => persistantDataPath + Path.DirectorySeparatorChar + "Analytics" + Path.DirectorySeparatorChar;
        static string confDest => analyticsPath + "settings.conf";
        static string zipname => $"{PlayerPrefs.GetString("player_name", "default")}_{from.ToString(dateFormat)}_{to.ToString(dateFormat)}.zip";
        static string zipDest => zipPath + zipname;

        public static void ZipAndSendData()
        {
            to = DateTime.UtcNow;
            FlushToFiles();
            ZipAndDeleteFolder();
            UploadZippedFile();
            from = to;
        }

        // Every death, win or on application quit
        public static void FlushToFiles()
        {
            meleeEventHandler.WriteToDisk(analyticsPath);
        }

        public static void ZipAndDeleteFolder()
        {
            if (!Directory.Exists(zipPath))
                Directory.CreateDirectory(zipPath);

            using (ZipFile zip = new ZipFile())
            {                
                zip.AddDirectory(analyticsPath);
                // add the report into a different directory in the archive
                zip.Save(zipDest);

                Debug.Log($"Zipped directory {analyticsPath} to {zipDest}");

                try
                {
                    Directory.Delete(analyticsPath, true);
                    Debug.Log($"Deleting directory {analyticsPath}");
                }
                catch
                {
                    Debug.Log($"Problem witch cleanup");
                }
            }
        }

        public static void UploadZippedFile()
        {
            SimpleHttpClient.UploadFile(zipDest, zipname, "application/zip");
        }

        public static void CreateConfig()
        {
            if (!Directory.Exists(analyticsPath))
                Directory.CreateDirectory(analyticsPath);

            StringBuilder b = new StringBuilder();
            b.AppendLine($"timeInterval={timeInterval}");
            b.AppendLine($"dateFormat={dateFormat}");
            b.AppendLine($"meleeData={meleeEventHandler.directory}");

            Debug.Log($"Creating config at {confDest}");

            File.WriteAllText(confDest, b.ToString());
        }
    }
}