using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Config;
using UnityEngine;

public class EnemyEnvDestroyer : EnemyAI
{

    private List<EnemyMelee> enemiesMelee;
    private List<EnemyRanged> enemiesRanged;

    [SerializeField]
    private GameObject rangedPrefab;

    [SerializeField]
    private GameObject meleePrefab;

    [SerializeField]
    private int enemiesMeleeMax;
    
    [SerializeField]
    private int enemiesRangedMax;

    private Tile target;

    [SerializeField]
    private MapController mapController;

    private bool coroutineRunning;

    private float minDistance;
    private float distanceTmp;
    private Tile tileTmp;

    public override void InitEnemy()
    {
        base.InitEnemy();
        enemiesMeleeMax = 1;
        enemiesRangedMax = 1;
        moveRangeMax = 8f;
        moveRangeMin = 5f;
        if (!mapController)
            mapController = FindObjectOfType<MapController>();
        coroutineRunning = false;
        idleDuration = 3;

        enemiesMelee = new List<EnemyMelee>();
        enemiesRanged = new List<EnemyRanged>();

        for (int i = 0; i < enemiesMeleeMax; i++)
        {
            GameObject enemy = null;
            enemy = Instantiate(meleePrefab);
            enemy.transform.position = EnemiesUtils.GetRoamPosition(transform.position, moveRangeMin, moveRangeMax);
            enemiesMelee.Add(enemy.GetComponent<EnemyMelee>());
            enemiesMelee[i].InitEnemy(transform);
            enemiesMelee[i].state = State.Roam;
        }

        for (int i = 0; i < enemiesRangedMax; i++)
        {
            GameObject enemy = null;
            enemy = Instantiate(rangedPrefab);
            enemy.transform.position = EnemiesUtils.GetRoamPosition(transform.position, moveRangeMin, moveRangeMax);
            enemiesRanged.Add(enemy.GetComponent<EnemyRanged>());
            enemiesRanged[i].InitEnemy(transform);
            enemiesRanged[i].state = State.Roam;
        }
        state = State.Idle;
    }

    protected override void ApplyConfig()
    {
        base.ApplyConfig();
        var enemyConfig = GlobalConfigManager.GetGlobalConfig().globalEnemyConfig.enemyEnvDestroyerConfig;
        enemiesMeleeMax = enemyConfig.enemiesMeleeMax;
        enemiesRangedMax = enemyConfig.enemiesRangedMax;
    }

    protected override EnemyConfig GetEnemyBaseConfig()
    {
        return GlobalConfigManager.GetGlobalConfig().globalEnemyConfig.enemyEnvDestroyerConfig.baseConfig;
    }

    protected override void Roam()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            state = State.Idle;
            agent.isStopped = true;
        }
    }


    protected override void Chase()
    {
        state = State.Roam;
    }

    protected override void Idle()
    {
        if (!coroutineRunning)
        {
            Attack();
            if (mapController.aliveTilesCnt == 0)
            {
                agent.SetDestination(EnemiesUtils.GetRoamPosition(transform.position, moveRangeMin, moveRangeMax));
                state = State.Roam;
                agent.isStopped = false;
            }
            else
            {
                coroutineRunning = true;
                minDistance = float.MaxValue;
                distanceTmp = 0f;
                tileTmp = null;
                FindTileToDestroyCoroutine();
            }
        }
    }
    
    async void FindTileToDestroyCoroutine()
    {
        for (int i = 0; i < mapController.tiles.Count; i++)
        {
            if (!mapController.tiles[i] || mapController.tiles[i].GetBiomeType() == BiomeType.DEAD || mapController.tiles[i].wantedType == BiomeType.DEAD || mapController.tiles[i].chosen)
                continue;
            distanceTmp = Vector3.Distance(transform.position, mapController.tiles[i].transform.position);
            if (distanceTmp < minDistance)
            { 
                minDistance = distanceTmp;
                tileTmp = mapController.tiles[i];
            }
        }

        tileTmp.chosen = true;
        
        await Task.Delay(Mathf.CeilToInt(idleDuration * 1000f));

        if (agent)
        {
            agent.SetDestination(tileTmp.transform.position);
            agent.isStopped = false;
        }
        state = State.Roam;
        coroutineRunning = false;
    }

    protected override void Attack()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, LayerMask.GetMask("Tile")))
        {
            Tile tile = hit.transform.gameObject.GetComponent<Tile>();
            if (!tile || tile.GetBiomeType() == BiomeType.DEAD)
                return;
            FMODUnity.RuntimeManager.PlayOneShot("event:/enemies/sucking/sucking");
            mapController.AttackTile(tile);
        }
    }

    public override void ReceiveDamage(float amount)
    {
        animator.SetTrigger("GetHit");

        if ((healthPoints -= amount) <= 0)
        {
            for (int i = 0; i < enemiesMelee.Count; i++)
            {
                enemiesMelee[i].SetRoamObjectTransform(null);
            }

            for (int i = 0; i < enemiesRanged.Count; i++)
            {
                enemiesRanged[i].SetRoamObjectTransform(null);
            }
            Die();
        }
    }

}
