using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameEvents
{
    public class GameStateChangedEvent : TimedEvent
    {
        public EGameState newState { get; private set; }
        public EGameState oldState { get; private set; }

        public GameStateChangedEvent(EGameState newState, EGameState oldState)
        {
            this.newState = newState;
            this.oldState = oldState;
        }
    }
}
