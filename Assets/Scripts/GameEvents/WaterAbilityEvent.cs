using UnityEngine;
using System.Collections;

namespace Assets.Scripts.GameEvents
{
    public class WaterAbilityEvent : TimedEvent
    {
        public bool cast { get; private set; }
        public bool pushedEnemy { get; private set; }
        public bool killedProjectile { get; private set; }

        public WaterAbilityEvent(bool cast = false, bool pushedEnemy = false, 
            bool killedProjectile = false)
        {
            this.cast = cast;
            this.pushedEnemy = pushedEnemy;
            this.killedProjectile = killedProjectile;
        }
    }
}