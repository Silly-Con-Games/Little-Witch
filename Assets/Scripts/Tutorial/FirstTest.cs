using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Tutorial
{
    public class FirstTest
    {
        public void Start()
        {
            var enContr = EnemiesController.instance;
            Assert.IsNotNull(enContr, "missing enemies controller");

            enContr.SetWave(0);
            enContr.SpawnNextWave();
            enContr.onWaveEnd.AddListener(Completed);

        }

        public void Reset()
        {
            var enContr = EnemiesController.instance;
            Assert.IsNotNull(enContr, "missing enemies controller");
            enContr.onWaveEnd.RemoveListener(Completed);
            enContr.KillAll();
            Start();
        }

        public void Completed()
        {
            EnemiesController.instance.onWaveEnd.RemoveListener(Completed);
        }
    }
}