using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameEvents
{
    public class EnemySpawnedEvent : TimedEvent
    {
        public EnemyType type { get; internal set; }
        public EnemySpawnedEvent(EnemyType type)
        {
            this.type = type; 
        }
    }
}
