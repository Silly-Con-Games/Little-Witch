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
        public void ResetEnemies()
        {
            KillAll();
            SpawnAll();
        }

        public void SpawnAll()
        {
            foreach(var t in enemiesToSpawn)
            {
                EnemyAI e = Instantiate(t.prefab);
                e.transform.position = t.trans.position;
                e.transform.rotation = t.trans.rotation;
                e.InitEnemy();
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
