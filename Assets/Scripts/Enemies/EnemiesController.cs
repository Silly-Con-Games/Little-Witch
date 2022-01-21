using Assets.Scripts.GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class EnemiesController : MonoBehaviour
{
    public List<WaveDefinition> waves;

    static int waveCounter = 0;

    public static EnemiesController instance;

    public UnityEvent onWaveEnd;


    int aliveEnemiesCnt = 0;

    public static int GetWaveCounter() => waveCounter;

    static void IncreaseAliveCount()
    {
        instance.aliveEnemiesCnt++;
    }

    static void DecreaseAliveCount()
    {
        instance.aliveEnemiesCnt--;

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
        GameEventQueue.AddListener(typeof(EnemySpawnedEvent), OnEnemySpawned);
        GameEventQueue.AddListener(typeof(EnemyDiedEvent), OnEnemyDied);
        waveCounter = 0;
    }

    private void OnDestroy()
    {
        instance = null;
        GameEventQueue.RemoveListener(typeof(EnemySpawnedEvent), OnEnemySpawned);
        GameEventQueue.RemoveListener(typeof(EnemyDiedEvent), OnEnemyDied);
    }

    void OnEnemySpawned(IGameEvent ev)
    {
        IncreaseAliveCount();
    }

    void OnEnemyDied(IGameEvent ev)
    {
        DecreaseAliveCount();
    }

    public void SetWave(int wave) {
		waveCounter = wave;
	}

    public bool WasLastWave()
    {
        return waveCounter == waves.Count;
    }

    public int GetRemainingWaves() => waves.Count - waveCounter;

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
