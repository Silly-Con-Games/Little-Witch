using UnityEngine;
using System.Collections;

namespace Assets.Scripts.GameEvents
{
    public class WaterAbilityEvent : IGameEvent
    {
        public float tStart { get; private set; }
        public bool cast { get; private set; }

        public WaterAbilityEvent(bool cast = false, bool pushedEnemy = false, 
            bool killedProjectile = false)
        {
            this.cast = cast;
            tStart = Time.time;
        }
    }
}