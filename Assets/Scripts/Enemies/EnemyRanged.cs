using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : EnemyAI
{

    public GameObject bulletPrefab;
    
    public override void InitEnemy()
    {
        base.InitEnemy();
        roamPosition = null;
    }

    protected override void Attack()
    {
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.GetComponentInChildren<Bullet>().target = EObjectType.Player;
        var bulletInstanceTrans = bullet.transform;
        bulletInstanceTrans.position = transform.position;
        bulletInstanceTrans.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    protected override void Idle()
    {
        if (IsPlayerInRange(maxRangeToPlayer))
        {
            state = State.Chase;
            idleTime = Random.Range(1f, 3f);
            return;
        }
        idleTime -= Time.deltaTime;
        if (idleTime <= 0)
        {
            state = State.Roam;
            agent.isStopped = false;
            agent.SetDestination(EnemiesUtils.GetRoamPosition(roamPosition.position, moveRangeMin, moveRangeMax));
            idleTime = Random.Range(1f, 3f);
        }
    }

    protected override void Roam()
    {
        if (IsPlayerInRange(maxRangeToPlayer))
        {
            state = State.Chase;
            return;
        }

        if (roamPosition == null)
        {
            state = State.Chase;
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agent.SetDestination(EnemiesUtils.GetRoamPosition(roamPosition.position, moveRangeMin, moveRangeMax));
        }
    }
    protected override void Chase()
    { 
        if (!IsPlayerInRange(maxRangeToPlayer))
        {
            if (chasingDeltaTime > 0 && playerController)
            {
                agent.SetDestination(playerController.transform.position);
                chasingDeltaTime -= Time.deltaTime;
                return;
            }
            if (roamPosition)
            {
                state = State.Roam;
                agent.isStopped = false;
                agent.SetDestination(EnemiesUtils.GetRoamPosition(roamPosition.position, moveRangeMin, moveRangeMax));
            }
            else
            {
                if (playerController)
                {
                    agent.SetDestination(playerController.transform.position);
                }
                else
                {
                    state = State.Roam;
                    roamPosition = transform;
                    agent.SetDestination(EnemiesUtils.GetRoamPosition(roamPosition.position, moveRangeMin, moveRangeMax));
                }
                agent.isStopped = false;
            }
            return;
        }

        if (!IsPlayerInRange(dashRange))
        {
            agent.isStopped = false;
            agent.SetDestination(playerController.transform.position);
        }
        else
        {
            attackTime -= Time.deltaTime;
            transform.LookAt(playerController.transform.position);
            if (attackTime < 0)
            {
                Attack();
                attackTime = attackSpeed;
            }
            agent.isStopped = true;
        }
    }

}
