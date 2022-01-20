using Assets.Scripts.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Enemies
{
    public class EnemiesGroupController : MonoBehaviour
    {
        [Serializable]
        public class Tuple
        {
            public EnemyAI prefab;
            public Transform trans;
        }

        public List<Tuple> enemiesToSpawn;

        List<EnemyAI> alive = new List<EnemyAI>();
        public UnityEvent groupDied;
        public void ResetEnemies(float spawnDelay = 0)
        {
            KillAll();
            StartCoroutine(CoroutineUtils.CallWithDelay(SpawnAll, spawnDelay));
        }

        public void SpawnAll()
        {
            foreach(var t in enemiesToSpawn)
            {
                EnemyAI e = Instantiate(t.prefab, t.trans.position, t.trans.rotation);
                alive.Add(e);
            }
            GameEventQueue.AddListener(typeof(EnemyDiedEvent), OnEnemyDied);
        }

        private void OnEnemyDied(IGameEvent e)
        {
            bool anyoneAlive = false;
            foreach(var en in alive)
                if (!en.IsDead)
                {
                    anyoneAlive = true;
                    break;
                }
            if (!anyoneAlive && alive.Count > 0)
            {
                groupDied.Invoke();
                GameEventQueue.RemoveListener(typeof(EnemyDiedEvent), OnEnemyDied);
            }
        }

        public void KillAll()
        {
            GameEventQueue.RemoveListener(typeof(EnemyDiedEvent), OnEnemyDied);
            foreach (var enemy in alive)
            {
                if (enemy != null && !enemy.IsDead)
                    enemy.ReceiveDamage(float.MaxValue);
            }
            alive = new List<EnemyAI>();
        }
    }
}
