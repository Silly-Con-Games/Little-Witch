using Assets.Scripts.Enemies;
using Assets.Scripts.GameEvents;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    public class FirstTest : MonoBehaviour
    {
        public EnemiesGroupController group;
        public OnTriggerEnterEvent ev;

        private void Awake()
        {
            ev.ontriggerenter.AddListener(PlayerEntered);
            group.groupDied.AddListener(Completed);
        }

        private void PlayerEntered(Collider other)
        {
            var ot = other.GetComponent<IObjectType>();
            if (ot != null && ot.GetObjectType() == EObjectType.Player) 
            {
                StartTest();
                ev.gameObject.SetActive(false);
            }
        }

        void OnPlayerRes(IGameEvent e)
        {
            ResetTest();
        }

        public void StartTest()
        {
            GameEventQueue.AddListener(typeof(PlayerRespawnedEvent), OnPlayerRes);
            group.SpawnAll();
        }

        public void ResetTest()
        {
            group.ResetEnemies();
        }

        public void Completed()
        {
            group.KillAll();
            GameEventQueue.RemoveListener(typeof(PlayerRespawnedEvent), OnPlayerRes);
            Debug.Log("First test completed");
        }
    }
}