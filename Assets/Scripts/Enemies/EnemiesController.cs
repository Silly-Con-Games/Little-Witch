using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class EnemiesController : MonoBehaviour
{
    public List<WaveDefinition> waves;

    int waveCounter = 0;

    private static EnemiesController instance;

    public UnityEvent onWaveEnd;

    static int aliveEnemiesCnt = 0;

    public static void IncreaseAliveCount()
    {
        aliveEnemiesCnt++;
    }

    public static void DecreaseAliveCount()
    {
        aliveEnemiesCnt--;
        if (aliveEnemiesCnt == 0)
            instance.onWaveEnd.Invoke();
    }


    private void Awake()
    {
        onWaveEnd = new UnityEvent();
        Assert.IsTrue(instance == null);
        instance = this;
    }

    public bool WasLastWave()
    {
        return waveCounter == waves.Count - 1;
    }

    public void SpawnNextWave()
    {
        Assert.IsTrue(waveCounter < waves.Count - 1);
        waves[waveCounter++].Spawn();
    }

    public float GetCurrentPreperationTime()
    {
        return waves[waveCounter].preparationTime;
    }
}
