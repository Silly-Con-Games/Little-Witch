namespace Assets.Scripts.GameEvents
{
    public class MeadowAbilityEvent : TimedEvent
    {
        public float damage { get; private set; }
        public bool cast { get; private set; }

        public MeadowAbilityEvent(float damage = 0, bool cast = false)
        {
            this.damage = damage;
            this.cast = cast;
        }
    }
}
