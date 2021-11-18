using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameEvents
{
    public class DashAbilityEvent : TimedEvent
    {
        public float distance { get; private set; }

        public DashAbilityEvent(float distance)
        {
            this.distance = distance;
        }
    }
}
