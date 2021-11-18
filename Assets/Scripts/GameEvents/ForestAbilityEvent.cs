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
        public float damagePath { get; private set; }
        public float damageEnd { get; private set; }
        public float rootDuration { get; private set; }

        public ForestAbilityEvent(bool cast = false, float damagePath = 0, float damageEnd = 0, float rootDuration = 0)
        {
            this.cast = cast;
            this.damagePath = damagePath;
            this.damageEnd = damageEnd;
            this.rootDuration = rootDuration;
        }
    }
}
