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

    void Start()
    {
        spawnDelay = 0.1f;
    }

    // spawns @number enemies of @enemyType type
    public void Spawn(EnemyType enemyType, int number)
    {
        StartCoroutine(SpawnEnemiesCoroutine(enemyType, number));
    }

    IEnumerator SpawnEnemiesCoroutine(EnemyType enemyType, int number)
    {
        for(int i = 0; i < number; ++i)
        {
            yield return new WaitForSeconds(spawnDelay);
            SpawnSingleEnemy(enemyType);
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
