using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        base.InitEnemy();
        enemiesMeleeMax = 3;
        enemiesRangedMax = 3;
        moveRangeMax = 20f;
        moveRangeMin = 15f;
        InitEnemy();
    }
    
    protected override void InitEnemy()
    {
        base.InitEnemy();
        enemiesMelee = new List<EnemyMelee>();
        enemiesRanged = new List<EnemyRanged>();

        for (int i = 0; i < enemiesMeleeMax; i++)
        {
            GameObject enemy = null;
            enemy = Instantiate(meleePrefab);
            enemy.transform.position = EnemiesUtils.GetRoamPosition(transform.position, moveRangeMin, moveRangeMax);
            enemiesMelee.Add(enemy.GetComponent<EnemyMelee>());
            enemiesMelee[i].SetRoamObjectTransform(this.transform);
        }

        for (int i = 0; i < enemiesRangedMax; i++)
        {
            GameObject enemy = null;
            enemy = Instantiate(rangedPrefab);
            enemy.transform.position = EnemiesUtils.GetRoamPosition(transform.position, moveRangeMin, moveRangeMax);
            enemiesRanged.Add(enemy.GetComponent<EnemyRanged>());
            enemiesRanged[i].SetRoamObjectTransform(this.transform);
        }
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
        idleTime -= Time.deltaTime;
        if (idleTime <= 0)
        {
            state = State.Roam;
            agent.isStopped = false;
            agent.SetDestination(EnemiesUtils.GetRoamPosition(transform.position, moveRangeMin, moveRangeMax));
            idleTime = Random.Range(1f, 3f);
            Attack();
        }
    }

    protected override void Attack()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, LayerMask.GetMask("Tile")))
        {
            Tile tile = hit.transform.parent.gameObject.GetComponent<Tile>();

            if (tile.tileState != TileState.DEAD)
            {
                tile.Die(false);
                foreach (Tile ngb in tile.GetNeighbours())
                {
                    if (ngb != null)
                    {
                        ngb.Die(false);
                    }
                }
            }
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
            Destroy(gameObject);
        }
    }
}
