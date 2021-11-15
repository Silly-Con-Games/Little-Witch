using Assets.Scripts.GameEvents;

namespace Assets.Scripts.Analytics
{
    public interface ITimedData<T> where T : IGameEvent
    {
        public void ProcessEvent(T e);
        public void Init(int snapshotIndex);
        public int GetSnapshotIndex();

        public bool NoEventsProcessed();
    }
}
