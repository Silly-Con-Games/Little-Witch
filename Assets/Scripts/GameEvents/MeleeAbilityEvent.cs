using UnityEngine;
namespace Assets.Scripts.GameEvents
{
    public class MeleeAbilityEvent : IGameEvent
    {
        public float tStart { get; private set; }
        public bool cast { get; private set; }
        public bool failedCast { get; private set; }
        public float damage { get; private set; }

        public MeleeAbilityEvent(bool cast = false, bool failedCast = false , float damage = 0)
        {
            this.cast = cast;
            this.failedCast = failedCast;
            this.damage = damage;
            tStart = Time.time;
        }
    }
}