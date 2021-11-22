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
    public class GameStateChangeEventHandler : IGameEventHandler
    {
        List<GameStateData> states = new List<GameStateData>();

        public GameStateChangeEventHandler()
        {
        }

        public Type GetEventType()
        {
            return typeof(GameStateChangedEvent);
        }

        public void HandleEvent(IGameEvent e)
        {
            GameStateChangedEvent ev = (GameStateChangedEvent)e;
            GameStateData data;
            data.newGameState = ev.newState.ToString();
            data.oldGameState= ev.oldState.ToString();
            data.time = ev.timeStart;
            data.wave = EnemiesController.GetWaveCounter();
            states.Add(data);
        }

        public string directory => typeof(GameStateData).Name;

        string fileName => $"GameStateData.json";

        public void WriteToDisk(string path)
        {
            path += directory + Path.DirectorySeparatorChar;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string filePath = path + fileName;

            Debug.Log($"Writing data to file {filePath}");

            string serialized = "[\n";

            for (int i = 0; i < states.Count; i++)
            {
                serialized += JsonUtility.ToJson(states[i], true) + ",\n";
            }
            serialized += "]";

            File.AppendAllText(filePath, serialized);

            states = new List<GameStateData>();
        }

    }
}
