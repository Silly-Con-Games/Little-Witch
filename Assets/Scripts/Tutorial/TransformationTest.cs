using Assets.Scripts.Enemies;
using Assets.Scripts.GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Tutorial
{
    public class TransformationTest : MonoBehaviour
    {
        public EnemiesGroupController group;
        public OnTriggerEnterEvent ev;
        public EAbilityType startHint = EAbilityType.None;
        public List<GameObject> energySpawners;
        public Transform tilesParent;
        public float transformationThreshold = 0.7f;

        public UnityEvent onCompleted;

        private List<Tile> tiles;

        private void Start()
        {
            ev.ontriggerenter.AddListener(PlayerEntered);

            tiles = new List<Tile>();
            for (int i = 0; i < tilesParent.childCount; i++)
            {
                Transform c = tilesParent.GetChild(i);
                Tile t = c.gameObject.GetComponent<Tile>();
                if (t != null)
                {
                    tiles.Add(t);
                }
            }
        }

        private void PlayerEntered(Collider other)
        {
            var ot = other.GetComponent<IObjectType>();
            if (ot != null && ot.GetObjectType() == EObjectType.Player)
            {
                StartTransformPart();
                ev.gameObject.SetActive(false);
            }
        }

        public void StartTransformPart()
        {
            Debug.Log($"Starting transform part of test {gameObject.name}", gameObject);
            GameEventQueue.AddListener(typeof(BiomeTransformedEvent), OnPlayerTransform);
            HintSpawner.SpawnHint(startHint);
        }

        public void StartCombatPart()
        {
            Debug.Log($"Starting combat part of test {gameObject.name}", gameObject);
            GameEventQueue.AddListener(typeof(PlayerRespawnedEvent), OnPlayerRes);
            foreach (var o in energySpawners)
                o.SetActive(false);
            group.groupDied.AddListener(Completed);
            group.SpawnAll();
        }

        void OnPlayerRes(IGameEvent e)
        {
            ResetCombatPartTest();
        }

        private void ResetCombatPartTest()
        {
            group.ResetEnemies();
        }

        private void OnPlayerTransform(IGameEvent ev)
        {
            BiomeTransformedEvent e = (BiomeTransformedEvent)ev;
            int aliveCnt = 0;
            foreach (var t in tiles)
            {
                if (t.GetBiomeType() != BiomeType.DEAD)
                    aliveCnt++;
            }

            if ((float)aliveCnt / tiles.Count > transformationThreshold)
            {
                GameEventQueue.RemoveListener(typeof(BiomeTransformedEvent), OnPlayerTransform);
                StartCombatPart();
            }
        }

        private void Completed()
        {
            Debug.Log($"Combat part of {gameObject.name} completed");
            onCompleted.Invoke();
        }
    }
}