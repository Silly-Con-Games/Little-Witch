using System.Collections;
using System.Collections.Generic;
using Config;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyMelee : EnemyAI
{
    public int threatLevel = 0;

    [SerializeField]
    private float dashDelay;

    private float dashDelta;
    
    [SerializeField]
    private float dashDuration;

    private float dashDurationDelta;

    [SerializeField]
    private float dashCooldown;
    
    private float dashCooldownDelta;

    [SerializeField]
    private float dashSpeedModifier;

    [SerializeField]
    protected float dashRange;

    private bool attacking;

    private Vector3 playerPos;
    private Vector3 lastPos;

    override protected void Update() 
    {
        base.Update();

        dashDelta -= Time.deltaTime;
        dashDurationDelta -= Time.deltaTime;
        dashCooldownDelta -= Time.deltaTime;
    }
    public override void InitEnemy()
    {
        base.InitEnemy();
        attacking = false;
    }

    protected override void ApplyConfig()
    {
        Assert.IsTrue(threatLevel < GlobalConfigManager.GetGlobalConfig().globalEnemyConfig.enemyMeleeConfigs.Count);
        base.ApplyConfig();
        var enemyConfig = GlobalConfigManager.GetGlobalConfig().globalEnemyConfig.enemyMeleeConfigs[threatLevel];
        dashDelay = enemyConfig.dashDelay;
        dashDuration = enemyConfig.dashDuration;
        dashRange = enemyConfig.dashRange;
        dashSpeedModifier = enemyConfig.dashSpeedModifier;
        dashCooldown = enemyConfig.dashCooldown;
        dashCooldownDelta = 0f;
    }

    protected override EnemyConfig GetEnemyBaseConfig()
    {
        Assert.IsTrue(threatLevel < GlobalConfigManager.GetGlobalConfig().globalEnemyConfig.enemyMeleeConfigs.Count);
        return GlobalConfigManager.GetGlobalConfig().globalEnemyConfig.enemyMeleeConfigs[threatLevel].baseConfig;
    }

    protected override void Idle()
    {
    }

    protected override void Roam()
    {
        if (IsPlayerInRange(maxRangeToPlayer))
        {
            state = State.Chase;
            return;
        }

        if (!roamPosition)
        {
            if (playerController)
            {
                state = State.Chase;
                return;
            }
            roamPosition = transform;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agent.SetDestination(EnemiesUtils.GetRoamPosition(roamPosition.position, moveRangeMin, moveRangeMax));
        }
    }
    protected override void Chase()
    {
        if (!playerController || (!IsPlayerInRange(maxRangeToPlayer) && chasingDeltaTime <= 0f && roamPosition))
        {
            state = State.Roam;
        } else if (IsPlayerInRange(dashRange) && dashCooldownDelta < 0)
        {
            dashDelta = dashDelay;
            dashDurationDelta = dashDuration;
            state = State.Attack;
        } else
        {
            if (IsPlayerInRange(attackRange) && agent.isActiveAndEnabled)
            {
                agent.isStopped = true;
                if (attackCooldownDelta <= 0f)
                {
                    animator.Attack();
                    playerController.ReceiveDamage(damage);
                    attackCooldownDelta = attackCooldown;
                }
                return;
            }

            if (agent.isActiveAndEnabled)
            {
                agent.isStopped = false;
                agent.SetDestination(playerController.transform.position);
            }
        }
    }

    // in scope of melee enemy attack means dash
    protected override void Attack()
    {
        if (!playerController)
        {
            attacking = false;
            state = State.Chase;
            agent.speed = speed;
            return;
        }

        if (agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
        }
        
        // waiting some time before dash
        while (dashDelta >= 0)
        {
            return;
        }

        if (agent.isActiveAndEnabled)
        {
            agent.speed = speed * dashSpeedModifier;
            agent.isStopped = false;
        }

        if (!attacking)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/enemies/dash/dash", transform.position);
            attacking = true;
        }

        while (agent.isActiveAndEnabled && dashDurationDelta >= 0 && !IsPlayerInRange(attackRange))
        {
            agent.SetDestination(playerController.transform.position);
            return;
        }

        if (IsPlayerInRange(attackRange))
        {
            animator.Attack();
            StartCoroutine(WaitAndTryDealDamage(0.6f));
        }

        attacking = false;
        state = State.Chase;
        agent.speed = speed;
        dashCooldownDelta = dashCooldown;
        attackCooldownDelta = attackCooldown;
    }

    IEnumerator WaitAndTryDealDamage(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (IsPlayerInRange(attackRange))
        {
            playerController.ReceiveDamage(damage);
        }
    }

}
