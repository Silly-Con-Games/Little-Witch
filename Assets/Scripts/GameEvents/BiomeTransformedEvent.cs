using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameEvents
{
    public class BiomeTransformedEvent : TimedEvent
    {
        public BiomeType from { get; private set; }
        public BiomeType to { get; private set; }
        public bool playerOrigin { get; private set; }
        public bool enemyOrigin { get; private set; }
        public bool revive { get; private set; }

        public float energyCost { get; private set; }
        public BiomeTransformedEvent(
            BiomeType from = BiomeType.UNKNOWN,
            BiomeType to = BiomeType.UNKNOWN,
            float energyCost = 0,
            bool playerOrigin = false, 
            bool enemyOrigin = false,
            bool revive = false)
        {
            this.energyCost = energyCost;
            this.from = from;
            this.to = to;
            this.playerOrigin = playerOrigin;
            this.enemyOrigin = enemyOrigin;
            this.revive = revive;
        }
    }
}
