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
        Debug.Log($"{aliveEnemiesCnt} enemies alive");
    }

    public static void DecreaseAliveCount()
    {
        aliveEnemiesCnt--;
        Debug.Log($"{aliveEnemiesCnt} enemies alive");

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
