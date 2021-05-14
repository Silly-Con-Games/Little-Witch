using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesController : MonoBehaviour
{

    [SerializeField]
    private List<EnemiesSpawn> enemiesSpawns;

    [SerializeField] 
    private IndicatorsCreator indicatorsCreator;
    
    // Start is called before the first frame update
    void Start()
    {

        enemiesSpawns[0].indicatorsCreator = indicatorsCreator;
        StartCoroutine(SpawnCourotine());
    }

    IEnumerator SpawnCourotine()
    {
        //enemiesSpawns[0].Spawn(EnemyType.EnvDestroyer, 2);

        enemiesSpawns[0].Spawn(EnemyType.Melee, 3);
        yield return new WaitForSeconds(15);
        enemiesSpawns[0].Spawn(EnemyType.Ranged, 2);
        enemiesSpawns[0].Spawn(EnemyType.Melee, 2);
        yield return new WaitForSeconds(15);
        enemiesSpawns[0].Spawn(EnemyType.Ranged, 2);
        enemiesSpawns[0].Spawn(EnemyType.Melee, 3);
        enemiesSpawns[0].Spawn(EnemyType.Bomber, 1);
        yield return new WaitForSeconds(15);
        //enemiesSpawns[0].Spawn(EnemyType.Ranged, 2);
        //enemiesSpawns[0].Spawn(EnemyType.Melee, 3);
        enemiesSpawns[0].Spawn(EnemyType.Bomber, 2);
        enemiesSpawns[0].Spawn(EnemyType.EnvDestroyer, 2);
    }
}
