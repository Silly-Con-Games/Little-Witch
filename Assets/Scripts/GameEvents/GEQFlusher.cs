using UnityEngine;

namespace Assets.Scripts.GameEvents
{
    public class GEQFlusher : MonoBehaviour
    {
        private void Update()
        {
            GameEventQueue.ProcessEvents();
        }
    }
}
