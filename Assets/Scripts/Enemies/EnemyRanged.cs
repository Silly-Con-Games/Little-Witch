using System.Collections;
using System.Collections.Generic;
using Config;
using UnityEngine;
using FMODUnity;

public class EnemyRanged : EnemyAI
{

    public GameObject bulletPrefab;

    public override void InitEnemy(IndicatorsCreator indicatorsCreator)
    {
        base.InitEnemy(indicatorsCreator);
        attackCooldownDelta = -1f;
    }

    protected override EnemyConfig GetEnemyBaseConfig()
    {
        return GlobalConfigManager.GetGlobalConfig().globalEnemyConfig.enemyRangedConfig.baseConfig;
    }

    protected override void Attack()
    {
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.GetComponentInChildren<Bullet>().target = EObjectType.Player;
        var bulletInstanceTrans = bullet.transform;
        bulletInstanceTrans.position = transform.position;
        bulletInstanceTrans.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);

        animator.SetTrigger("Attack");

        FMOD.Studio.EventInstance instance = RuntimeManager.CreateInstance("event:/test/shot");
		RuntimeManager.AttachInstanceToGameObject(instance, transform, GetComponent<Rigidbody>());
		instance.setParameterByName("shot_pitch", Random.Range(-1f, 1f));
		instance.start();
		instance.release();
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

        if (!IsPlayerInRange(attackRange))
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
            agent.isStopped = true;
        }
    }

}
