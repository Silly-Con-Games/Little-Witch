using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    [SerializeField]
    private int idleTimeMillis;

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
        idleTimeMillis = 3000;

        enemiesMelee = new List<EnemyMelee>();
        enemiesRanged = new List<EnemyRanged>();

        for (int i = 0; i < enemiesMeleeMax; i++)
        {
            GameObject enemy = null;
            enemy = Instantiate(meleePrefab);
            enemy.transform.position = EnemiesUtils.GetRoamPosition(transform.position, moveRangeMin, moveRangeMax);
            enemiesMelee.Add(enemy.GetComponent<EnemyMelee>());
            enemiesMelee[i].InitEnemy(this.transform);
            enemiesMelee[i].state = State.Roam;
        }

        for (int i = 0; i < enemiesRangedMax; i++)
        {
            GameObject enemy = null;
            enemy = Instantiate(rangedPrefab);
            enemy.transform.position = EnemiesUtils.GetRoamPosition(transform.position, moveRangeMin, moveRangeMax);
            enemiesRanged.Add(enemy.GetComponent<EnemyRanged>());
            enemiesRanged[i].InitEnemy(this.transform);
            enemiesRanged[i].state = State.Roam;
        }
        state = State.Idle;
    }

    protected override void Roam()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            state = State.Idle;
            agent.isStopped = true;
        }
    }

    protected override void Chase(){}

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
            if (!mapController.tiles[i])
                continue;
            distanceTmp = Vector3.Distance(transform.position, mapController.tiles[i].transform.position);
            if (distanceTmp < minDistance)
            { 
                minDistance = distanceTmp;
                tileTmp = mapController.tiles[i];
            }
        }

        await Task.Delay(idleTimeMillis);

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
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            Tile tile = hit.transform.parent.gameObject.GetComponent<Tile>();
            if (!tile || tile.tileState == TileState.DEAD || tile.tileState == TileState.DYING)
                return;
            
            mapController.AttackTile(tile);
        }
    }

    public override void ReceiveDamage(float amount)
    {
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
