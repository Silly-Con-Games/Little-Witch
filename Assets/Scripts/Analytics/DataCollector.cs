using Assets.Scripts.GameEvents;
using System.IO;
using UnityEngine;
using Ionic.Zip;
using System;
using System.Text;
using System.Collections.Generic;

namespace Assets.Scripts.Analytics
{
    public static class DataCollector 
    {
        static float timeInterval = 5;
        static List<IGameEventHandler> handlers = new List<IGameEventHandler>();
        public static readonly string dateFormat = "yyyy-MM-ddTHH-mm-ss.f";
        static DateTime from = DateTime.UtcNow;
        static DateTime to;

        static bool initialized = false;
        [RuntimeInitializeOnLoadMethod]
        static void RunOnStart()
        {
            if (!initialized)
            {
                initialized = true;
                persistantDataPath = Application.persistentDataPath;
                Application.quitting += OnApplicationQuit_Internal;
                CreateHandlers();
                CreateConfig();

                foreach (var handler in handlers)
                    GameEventQueue.AddListener(handler.GetEventType(), handler.HandleEvent);

                GameController.onGameStateChanged.AddListener(FlushOnGameWonOrOver);
            }
            
        }

        static void CreateHandlers()
        {
            handlers.Add(new TimedEventHandler<MeleeAbilityEvent, MeleeData>(timeInterval));
            handlers.Add(new TimedEventHandler<WaterAbilityEvent, WaterAbilityData>(timeInterval));
            handlers.Add(new TimedEventHandler<MainAbilityFailEvent, MainAbilityFailData>(timeInterval));
            handlers.Add(new TimedEventHandler<ForestAbilityEvent, ForestAbilityData>(timeInterval));
            handlers.Add(new TimedEventHandler<MeadowAbilityEvent, MeadowAbilityData>(timeInterval));
            handlers.Add(new TimedEventHandler<DashAbilityEvent, DashData>(timeInterval));
        }

        private static void OnApplicationQuit_Internal()
        {
            foreach (var handler in handlers)
                GameEventQueue.RemoveListener(handler.GetEventType(), handler.HandleEvent);
            GameController.onGameStateChanged.RemoveListener(FlushOnGameWonOrOver);

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
            foreach (var handler in handlers)
                handler.WriteToDisk(analyticsPath);
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
#if UNITY_EDITOR
            Debug.Log("Not sending files to server inside editor, should work in standalone tho");
#else
            SimpleHttpClient.UploadFile(zipDest, zipname, "application/zip");
#endif
        }

        public static void CreateConfig()
        {
            if (!Directory.Exists(analyticsPath))
                Directory.CreateDirectory(analyticsPath);

            StringBuilder b = new StringBuilder();
            b.AppendLine($"timeInterval={timeInterval}");
            b.AppendLine($"dateFormat={dateFormat}");

            Debug.Log($"Creating config at {confDest}");

            File.WriteAllText(confDest, b.ToString());
        }
    }
}