using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : EnemyAI
{
    
    [SerializeField]
    private float dashDelay;

    private float dashDelta;
    
    [SerializeField]
    private float dashDuration;

    private float dashDurationDelta;

    [SerializeField]
    private float dashSpeedModifier;
    
    private bool attacking;

    private Vector3 playerPos;
    private Vector3 lastPos;
    
    public override void InitEnemy()
    {
        dashDelay = 1f;
        dashDuration = 2f;
        attacking = false;
        dashSpeedModifier = 3f;
        base.InitEnemy();
        roamPosition = null;
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
            ;
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
            transform.LookAt(playerController.transform.position);
            state = State.Attack;
        }
    }

    protected override void Attack()
    {
        attackTime -= Time.deltaTime;
        if (!attacking && playerController && attackTime < 0)
        {
            agent.isStopped = true;
            attacking = true;
            attackTime = attackSpeed;
            dashDelta = dashDelay;
            dashDurationDelta = dashDuration;
            lastPos = transform.position;
            playerPos = playerController.transform.position;
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        while (dashDelta >= 0)
        {
            dashDelta -= Time.deltaTime;
            yield return null;
        }

        agent.speed = agent.speed * dashSpeedModifier;
        agent.isStopped = false;
        while (playerController && dashDurationDelta >= 0 && !IsCloseToAttack())
        {
            agent.SetDestination(playerController.transform.position);
            dashDurationDelta -= Time.deltaTime;
            yield return null;
        }

        if (IsCloseToAttack())
        {
            animator.SetTrigger("Attack");
            playerController.ReceiveDamage(3);
        }

        attacking = false;
        state = State.Chase;
        agent.speed = speed;
        agent.isStopped = false;
        agent.SetDestination(playerController.transform.position);
    }

}
