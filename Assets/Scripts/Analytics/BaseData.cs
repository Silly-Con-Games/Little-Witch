using Assets.Scripts.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Analytics
{
    public abstract class BaseData
    {
        public abstract void HandleEvent(IGameEvent e);
        public abstract Type GetEventType();
    }
}
