using Assets.Scripts.GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Analytics
{
    public class DataCollector : MonoBehaviour
    {
        MeleeData meleeData = new MeleeData();
        // Start is called before the first frame update
        void Start()
        {
            GameEventQueue.AddListener(meleeData.GetEventType(), meleeData.HandleEvent);
        }

        private void OnDestroy()
        {
            GameEventQueue.RemoveListener(meleeData.GetEventType(), meleeData.HandleEvent);
        }
    }
}