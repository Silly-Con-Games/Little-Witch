using System.Collections;
using System.Collections.Generic;
using Config;
using UnityEngine;

public class EnemyMelee : EnemyAI
{
    
    [SerializeField]
    private float dashDelay;

    private float dashDelta;
    
    [SerializeField]
    private float dashDuration;

    private float dashDurationDelta;

    // TODO: Add this field to config
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

    protected void Update()
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
        base.ApplyConfig();
        var enemyConfig = GlobalConfigManager.GetGlobalConfig().globalEnemyConfig.enemyMeleeConfig;
        dashDelay = enemyConfig.dashDelay;
        dashDuration = enemyConfig.dashDuration;
        dashRange = enemyConfig.dashRange;
        dashSpeedModifier = enemyConfig.dashSpeedModifier;
        dashCooldown = 5f;
        dashCooldownDelta = 0f;
    }

    protected override EnemyConfig GetEnemyBaseConfig()
    {
        return GlobalConfigManager.GetGlobalConfig().globalEnemyConfig.enemyMeleeConfig.baseConfig;
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
            if (IsPlayerInRange(attackRange))
            {
                agent.isStopped = true;
                if (attackCooldownDelta <= 0f)
                {
                    animator.SetTrigger("Attack");
                    playerController.ReceiveDamage(3);
                    attackCooldownDelta = attackCooldown;
                }
                return;
            }
            agent.isStopped = false;
            agent.SetDestination(playerController.transform.position);
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
        
        agent.isStopped = true;
        // waiting some time before dash
        while (dashDelta >= 0)
        {
            return;
        }
        agent.speed = speed * dashSpeedModifier;
        agent.isStopped = false;

        if (!attacking)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/enemies/dash/dash");
            attacking = true;
        }

        while (dashDurationDelta >= 0 && !IsPlayerInRange(attackRange))
        {
            agent.SetDestination(playerController.transform.position);
            return;
        }

        if (IsPlayerInRange(attackRange))
        {
            animator.SetTrigger("Attack");
            playerController.ReceiveDamage(1);
        }

        attacking = false;
        state = State.Chase;
        agent.speed = speed;
        dashCooldownDelta = dashCooldown;
        attackCooldownDelta = attackCooldown;
    }

}
