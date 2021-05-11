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

        for (int i = 0; i < enemiesSpawns.Count; i++)
        {
            enemiesSpawns[i].indicatorsCreator = indicatorsCreator;
        }
        
        //enemiesSpawns[0].Spawn(EnemyType.Melee, 2);
        enemiesSpawns[0].Spawn(EnemyType.Ranged, 3);
        enemiesSpawns[1].Spawn(EnemyType.Bomber, 2);
        enemiesSpawns[2].Spawn(EnemyType.EnvDestroyer, 5);
        enemiesSpawns[3].Spawn(EnemyType.Melee,1);
    }
}
