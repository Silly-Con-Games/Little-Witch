using Assets.Scripts.GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Analytics
{
    public class DataCollector : MonoBehaviour
    {
        TimedEventHandler<MeleeAbilityEvent, MeleeData> meleeEventHandler = new TimedEventHandler<MeleeAbilityEvent, MeleeData>();
        // Start is called before the first frame update
        void Start()
        {
            GameEventQueue.AddListener(meleeEventHandler.GetEventType(), meleeEventHandler.HandleEvent);
        }

        private void OnDestroy()
        {
            GameEventQueue.RemoveListener(meleeEventHandler.GetEventType(), meleeEventHandler.HandleEvent);
        }
    }
}