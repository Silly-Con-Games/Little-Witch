using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class EnemiesController : MonoBehaviour
{
    public List<WaveDefinition> waves;

    int waveCounter = 3;

    private static EnemiesController instance;

    public UnityEvent onWaveEnd;

    int aliveEnemiesCnt = 0;


    public static void IncreaseAliveCount()
    {
        instance.aliveEnemiesCnt++;
        Debug.Log($"{instance.aliveEnemiesCnt} enemies alive");
    }

    public static void DecreaseAliveCount()
    {
        instance.aliveEnemiesCnt--;
        Debug.Log($"{instance.aliveEnemiesCnt} enemies alive");

        if (instance.aliveEnemiesCnt == 0)
        {
            instance.onWaveEnd.Invoke();
        }
    }


    private void Awake()
    {
        onWaveEnd = new UnityEvent();
        Assert.IsTrue(instance == null);
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public bool WasLastWave()
    {
        return waveCounter == waves.Count;
    }

    public void SpawnNextWave()
    {
        Assert.IsTrue(waveCounter < waves.Count);
        aliveEnemiesCnt = 0;
        waves[waveCounter++].Spawn();
    }

    public float GetCurrentPreperationTime()
    {
        return waves[waveCounter].preparationTime;
    }
}
