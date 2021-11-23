using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameEvents
{
    public class BiomeTransformationFailedEvent : TimedEvent
    {
        public bool noEnergy { get; private set; }
        public bool invalidTile { get; private set; }
        public bool revive { get; private set; }

        public BiomeTransformationFailedEvent(bool noEnergy = false, bool invalidTile = false, bool revive = false)
        {
            this.noEnergy = noEnergy;
            this.invalidTile = invalidTile;
            this.revive = revive;
        }
    }
}
