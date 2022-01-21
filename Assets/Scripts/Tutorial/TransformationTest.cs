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
        private BiomeType[] backUp;
        private HUDController hud;
        private void Start()
        {
            ev.ontriggerenter.AddListener(PlayerEntered);
            hud = FindObjectOfType<HUDController>();
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
            backUp = new BiomeType[tiles.Count];
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
            hud.ShowHintText($"Transform {transformationThreshold*100}% of the tiles");
            GameEventQueue.AddListener(typeof(BiomeTransformedEvent), OnPlayerTransform);
            HintSpawner.SpawnHint(startHint);
        }

        public void StartCombatPart()
        {
            hud.ShowHintText("Prepare to fight!");
            Debug.Log($"Starting combat part of test {gameObject.name}", gameObject);
            GameEventQueue.AddListener(typeof(PlayerRespawnedEvent), OnPlayerRes);
            foreach (var o in energySpawners)
                o.SetActive(false);
            group.groupDied.AddListener(Completed);
            StartCoroutine(CoroutineUtils.CallWithDelay(group.SpawnAll, 4));
        }

        void OnPlayerRes(IGameEvent e)
        {
            ResetCombatPartTest();
        }

        private void ResetCombatPartTest()
        {
            hud.ShowHintText("Prepare to fight!");
            FindObjectOfType<PlayerController>().energy.AddEnergy(1000);
            for (int i = 0; i < backUp.Length; i++)
                tiles[i].Morph(backUp[i], true);
            group.ResetEnemies(3);
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
                for (int i = 0; i < backUp.Length; i++)
                    backUp[i] = tiles[i].GetBiomeType();
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