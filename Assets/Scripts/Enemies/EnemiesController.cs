using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesController : MonoBehaviour
{

    [SerializeField]
    private List<EnemiesSpawn> enemiesSpawns;
    
    // Start is called before the first frame update
    void Start()
    {
        //enemiesSpawns[0].Spawn(EnemyType.Melee, 2);
        enemiesSpawns[0].Spawn(EnemyType.Ranged, 1);
        enemiesSpawns[1].Spawn(EnemyType.Bomber, 2);
        enemiesSpawns[2].Spawn(EnemyType.EnvDestroyer, 5);
        enemiesSpawns[3].Spawn(EnemyType.Melee,1);
    }
}
