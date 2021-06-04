using System.Collections;
using System.Collections.Generic;
using Config;
using UnityEngine;
using FMODUnity;
using UnityEngine.Assertions;

public class EnemyRanged : EnemyAI
{
    public int threatLevel;
    public GameObject bulletPrefab;

    public override void InitEnemy()
    {
        base.InitEnemy();
        attackCooldownDelta = -1f;
    }

    protected override EnemyConfig GetEnemyBaseConfig()
    {
        Assert.IsTrue(threatLevel < GlobalConfigManager.GetGlobalConfig().globalEnemyConfig.enemyRangedConfigs.Count);
        return GlobalConfigManager.GetGlobalConfig().globalEnemyConfig.enemyRangedConfigs[threatLevel].baseConfig;
    }

    protected override void Attack()
    {
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.GetComponentInChildren<Bullet>().target = EObjectType.Player;
        var bulletInstanceTrans = bullet.transform;
        bulletInstanceTrans.position = transform.position;
        bulletInstanceTrans.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);

        animator.Attack();

        FMODUnity.RuntimeManager.PlayOneShot("event:/enemies/shot/ranged_shot");
    }

    protected override void Idle()
    {
        if (IsPlayerInRange(maxRangeToPlayer))
        {
            state = State.Chase;
            idleDuration = Random.Range(1f, 3f);
            return;
        }
        idleDuration -= Time.deltaTime;
        if (idleDuration <= 0)
        {
            state = State.Roam;
            agent.isStopped = false;
            agent.SetDestination(EnemiesUtils.GetRoamPosition(roamPosition.position, moveRangeMin, moveRangeMax));
            idleDuration = Random.Range(1f, 3f);
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

        if (agent.isActiveAndEnabled && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agent.SetDestination(EnemiesUtils.GetRoamPosition(roamPosition.position, moveRangeMin, moveRangeMax));
        }
    }
    protected override void Chase()
    { 
        if (!IsPlayerInRange(maxRangeToPlayer))
        {
            if (chasingDeltaTime > 0 && playerController && agent.isActiveAndEnabled)
            {
                agent.SetDestination(playerController.transform.position);
                chasingDeltaTime -= Time.deltaTime;
                return;
            }
            if (roamPosition && agent.isActiveAndEnabled)
            {
                state = State.Roam;
                agent.isStopped = false;
                agent.SetDestination(EnemiesUtils.GetRoamPosition(roamPosition.position, moveRangeMin, moveRangeMax));
            }
            else
            {
                if (playerController && agent.isActiveAndEnabled)
                {
                    agent.SetDestination(playerController.transform.position);
                }
                else if (agent.isActiveAndEnabled)
                {
                    state = State.Roam;
                    roamPosition = transform;
                    agent.SetDestination(EnemiesUtils.GetRoamPosition(roamPosition.position, moveRangeMin, moveRangeMax));
                }

                if (agent.isActiveAndEnabled)
                {
                    agent.isStopped = false;
                }
            }
            return;
        }

        if (!IsPlayerInRange(attackRange) && agent.isActiveAndEnabled)
        {
            agent.isStopped = false;
            agent.SetDestination(playerController.transform.position);
        }
        else
        {
            attackCooldownDelta -= Time.deltaTime;
            transform.LookAt(playerController.transform.position);
            if (attackCooldownDelta < 0)
            {
                Attack();
                attackCooldownDelta = attackCooldown;
            }

            if (agent.isActiveAndEnabled)
            {
                agent.isStopped = true;
            }
        }
    }

}
