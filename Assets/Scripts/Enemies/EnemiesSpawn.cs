using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesSpawn : MonoBehaviour
{

    [SerializeField]
    private float spawnRange;

    [SerializeField]
    private float spawnDelay;

    [SerializeField]
    private GameObject rangedPrefab;
    
    [SerializeField]
    private GameObject meleePrefab;
    
    [SerializeField]
    private GameObject bomberPrefab;
    
    [SerializeField]
    private GameObject envDestroyerPrefab;

    private float spawnDelta;
    
    private Queue<EnemyAI> enemiesQueue;

    private float TEST_DELTA;
    
    void Start()
    {
        spawnDelay = 5f; 
    }

    // spawns @number enemies of @enemyType type
    public void Spawn(EnemyType enemyType, int number)
    {
        StartCoroutine(SpawnEnemiesCoroutine(enemyType, number));
    }

    IEnumerator SpawnEnemiesCoroutine(EnemyType enemyType, int number)
    {
        while (number > 0)
        {
            spawnDelta = spawnDelay;
            while (spawnDelta >= 0)
            {
                spawnDelta -= Time.deltaTime;
                yield return null;
            }
            SpawnSingleEnemy(enemyType);
            number--;
        }
    }

    private void SpawnSingleEnemy(EnemyType enemyType)
    {
        GameObject enemy = null;
        
        switch (enemyType)
        {
            case EnemyType.Ranged:
                enemy = Instantiate(
                    rangedPrefab,
                    EnemiesUtils.GetRoamPosition(transform.position, 0f, spawnRange),
                    transform.rotation
                );
                enemy.GetComponent<EnemyRanged>().InitEnemy();
                break;
            case EnemyType.Melee:
                enemy = Instantiate(
                    meleePrefab,
                    EnemiesUtils.GetRoamPosition(transform.position, 0f, spawnRange),
                    transform.rotation
                );
                enemy.GetComponent<EnemyMelee>().InitEnemy();
                break;
            case EnemyType.Bomber:
                enemy = Instantiate(
                    bomberPrefab,
                    EnemiesUtils.GetRoamPosition(transform.position, 0f, spawnRange),
                    transform.rotation
                );
                enemy.GetComponent<EnemyBomber>().InitEnemy();
                break;
            case EnemyType.EnvDestroyer:
                enemy = Instantiate(
                    envDestroyerPrefab,
                    EnemiesUtils.GetRoamPosition(transform.position, 0f, spawnRange),
                    transform.rotation
                );
                enemy.GetComponent<EnemyEnvDestroyer>().InitEnemy();
                break;
        }
        
    }





}
