using UnityEngine;
namespace Assets.Scripts.GameEvents
{
    public class MeleeAbilityEvent : TimedEvent
    {
        public bool cast { get; private set; }
        public bool failedCast { get; private set; }
        public float damageDealt { get; private set; }

        public MeleeAbilityEvent(bool cast = false, bool failedCast = false , float damage = 0)
        {
            this.cast = cast;
            this.failedCast = failedCast;
            this.damageDealt = damage;
        }
    }
}