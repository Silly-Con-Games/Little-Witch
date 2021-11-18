using Assets.Scripts.GameEvents;
using System;

namespace Assets.Scripts.Analytics
{
    public interface IGameEventHandler
    {
        public void HandleEvent(IGameEvent e);
        public Type GetEventType();
        public void WriteToDisk(string path);
    }
}
