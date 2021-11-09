using UnityEngine;

namespace Assets.Scripts.GameEvents
{
    public class TimedEvent : IGameEvent
    {
        // Time in seconds at start of the frame in which this event was recorded,
        // could be effected by timescale and pauses
        public float timeStart { get; private set; }

        // Real time in seconds at start of the frame in which this event was recorded
        public float realTimeStart { get; private set; }

        public TimedEvent()
        {
            timeStart = Time.time;
            realTimeStart = Time.unscaledTime;
        }
    }
}
