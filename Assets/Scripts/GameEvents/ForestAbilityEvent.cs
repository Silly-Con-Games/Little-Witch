using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameEvents
{
    class ForestAbilityEvent : TimedEvent
    {
        public bool cast { get; private set; }
        public float damage { get; private set; }
        public float rootDuration { get; private set; }

        public ForestAbilityEvent(bool cast = false, float damage = 0, float rootDuration = 0)
        {
            this.cast = cast;
            this.damage = damage;
            this.rootDuration = rootDuration;
        }
    }
}
