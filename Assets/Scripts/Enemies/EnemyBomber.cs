using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomber : EnemyAI
{

    [SerializeField]
    private GameObject bombPrefab;

    public override void InitEnemy()
    {
        base.InitEnemy();
        if (playerController)
            roamPosition = playerController.transform;
        else
            roamPosition = transform;
        agent.isStopped = false;
        agent.SetDestination(EnemiesUtils.GetRoamPosition(roamPosition.position, moveRangeMin, moveRangeMax));
    }
    
    protected override void Roam()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agent.isStopped = true;
            state = State.Idle;
        }
    }

    protected override void Chase()
    {
    }
    
    protected override void Idle()
    {
        idleTime -= Time.deltaTime;
        if (idleTime <= 0)
        {
            state = State.Roam;
            agent.isStopped = false;
            if (roamPosition == null)
            {
                roamPosition = transform;
            }
            agent.SetDestination(EnemiesUtils.GetRoamPosition(roamPosition.position, moveRangeMin, moveRangeMax));
            idleTime = Random.Range(5f, 8f);
            Attack();
        }
    }

    protected override void Attack()
    {
        GameObject bomb = Instantiate(bombPrefab);
        bomb.transform.position = this.transform.position;
    }

}
