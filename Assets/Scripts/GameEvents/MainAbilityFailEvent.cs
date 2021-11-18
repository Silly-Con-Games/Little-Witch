using UnityEngine;
using System.Collections;

namespace Assets.Scripts.GameEvents
{
    public class MainAbilityFailEvent : TimedEvent
    {
        public bool notOnCd { get; private set; }
        public bool deadBiome { get; private set; }

        public MainAbilityFailEvent(bool notOnCd = false, bool deadBiome = false)
        {
            this.notOnCd = notOnCd;
            this.deadBiome = deadBiome;
        }
    }
}