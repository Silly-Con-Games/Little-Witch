using Assets.Scripts.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Analytics
{
    public class MeleeData : BaseData
    {
        public override Type GetEventType()
        {
            return typeof(MeleeAbilityEvent);
        }

        public override void HandleEvent(IGameEvent e)
        {
            MeleeAbilityEvent ev = (MeleeAbilityEvent)e;
        }
    }
}
