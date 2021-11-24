using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameEvents
{
    public class ChargeAbilityEvent : TimedEvent
    {
        public bool cast { get; private set; }
        public bool failCast { get; private set; }
        public float damage { get; private set; }
        public float energyCost { get; private set; }

        public ChargeAbilityEvent(bool cast = false, bool failCast = false, float damage = 0, float energyCost = 0)
        {
            this.cast = cast;
            this.failCast = failCast;
            this.damage = damage;
            this.energyCost = energyCost;
        }
    }
}
