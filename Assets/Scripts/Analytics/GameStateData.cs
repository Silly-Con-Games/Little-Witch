using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Analytics
{
    [Serializable]
    public struct GameStateData
    {
        public string oldGameState;
        public string newGameState;
        public int wave;
        public float time; 
    }
}
