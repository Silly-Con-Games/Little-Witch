using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.GameEvents;
using Config;
using UnityEngine;
using UnityEngine.AI;

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
        type = EnemyType.EnvDestroyer;
        base.InitEnemy();

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
            enemiesMelee[i].state = State.Roam;
        }

        for (int i = 0; i < enemiesRangedMax; i++)
        {
            GameObject enemy = null;
            enemy = Instantiate(rangedPrefab);
            enemy.transform.position = EnemiesUtils.GetRoamPosition(transform.position, moveRangeMin, moveRangeMax);
            enemiesRanged.Add(enemy.GetComponent<EnemyRanged>());
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
        if (agent.isActiveAndEnabled && !agent.pathPending && agent.remainingDistance < 0.1f)
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
            // because one tile(under the home tree) is unreachable
            if (mapController.aliveTilesCnt <= 1)
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
        NavMeshPath path = new NavMeshPath();
        foreach (var tile in mapController.morphableTiles)
        {
            if (
                    tile.IsDead ||
                    ! tile.CanBeMorphed() ||
                    tile.wantedType == BiomeType.DEAD ||
                    tile.chosen
               )
                continue;
            

            distanceTmp = Vector3.Distance(transform.position, tile.transform.position);
            if (distanceTmp < minDistance)
            {
                if (agent.CalculatePath(tile.transform.position, path))
                {
                    minDistance = distanceTmp;
                    tileTmp = tile;
                }
            }
        }

        if (tileTmp)
        {
            Debug.Log("Tile found", tileTmp);
            tileTmp.chosen = true;
        }
        else
        {
            state = State.Roam;
            coroutineRunning = false;
            return;
        }
        
        await Task.Delay(Mathf.CeilToInt(idleDuration * 1000f));

        if (agent && agent.isActiveAndEnabled)
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
            if (!tile || tile.IsDead)
                return;
            GameEventQueue.QueueEvent(new BiomeTransformedEvent(from: tile.GetBiomeType(), to: BiomeType.DEAD, enemyOrigin: true));
            FMODUnity.RuntimeManager.PlayOneShot("event:/enemies/sucking/sucking", transform.position);
            tile.Morph(BiomeType.DEAD, false);
        }
    }

    public override void ReceiveDamage(float amount)
    {
        healthPoints -= amount;
        animator.GetHit();
        FMODUnity.RuntimeManager.PlayOneShot("event:/enemies/hit/generic_hit", transform.position);
        if (!healthbar.gameObject.activeSelf) healthbar.gameObject.SetActive(true);
        healthbar.value = healthPoints;
        if (healthPoints <= 0)
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
