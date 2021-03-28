using UnityEngine;
using System;
using UnityEngine.Events;
using System.IO;
using Config;

namespace Config
{

    [Serializable]
    public class GlobalConfigManager : MonoBehaviour
    {
        #region Instance

        [Tooltip("If no name is specified for load/save config function, this one will be used")]
        public string DefaultConfigFileName = "defaultConfig.json";

        [Button("LoadConfig", "Load config", true)] public string loadFileName;
        public void LoadConfig(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                filename = DefaultConfigFileName;

            string filePath = Application.dataPath + Path.DirectorySeparatorChar + "Config" + Path.DirectorySeparatorChar + filename;
            Debug.Log($"Loading from {filePath}");

            string json = File.ReadAllText(filePath);

            globalConfig = JsonUtility.FromJson<GlobalConfig>(json);

            onConfigChanged.Invoke();
        }

        [Button("SaveConfig", "Save config", true)] public string saveFileName;
        public void SaveConfig(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                filename = DefaultConfigFileName;

            string filePath = Application.dataPath + Path.DirectorySeparatorChar + "Config" + Path.DirectorySeparatorChar + filename;

            string json = JsonUtility.ToJson(globalConfig, true);

            Debug.Log($"Saving to {filePath}");

            File.WriteAllText(filePath, json);
        }

        public void OnValidate()
        {
            onConfigChanged.Invoke();
        }

        public GlobalConfig globalConfig;

        private void Awake()
        {
            instance = this;
        }

        #endregion Instance

        #region Static 

        static GlobalConfigManager instance;

        public static UnityEvent onConfigChanged = new UnityEvent();

        public static WitchConfig GetWitchConfig()
        {
            return instance.globalConfig.witchConfig;
        }

        public static GlobalConfig GetGlobalConfig()
        {
            return instance.globalConfig;
        }

        #endregion Static
    }
}