using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public abstract class EnemyAI : MonoBehaviour, IDamagable
{
    enum State
    {
        Roam,
        Chase,
        Idle
    }

    enum SecondaryState
    {
        Stun,
        Root,
        Slow,
        None
    }

    State state;
    private SecondaryState secondaryState;

    //[SerializeField]
    //private float stunTime;

    private float stunDeltaTime;

    [SerializeField]
    private float idleTime;

    [SerializeField]
    private float attackTime;

    [SerializeField]
    private float chasingTime;

    private float chasingDeltaTime;

    private NavMeshAgent agent;

    private Vector3 startingPosition;

    [SerializeField]
    protected PlayerController playerController;

    [SerializeField]
    private float moveRangeMin;

    [SerializeField]
    private float moveRangeMax;

    [SerializeField]
    private float maxRangeToPlayer;

    [SerializeField]
    private float attackRange;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float attackSpeed;

    [SerializeField]
    private float healthPoints;

    void Start()
    {
        if (!playerController)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
        startingPosition = transform.position;
        moveRangeMin = 4f;
        moveRangeMax = 4f;
        maxRangeToPlayer = 8f;

        idleTime = -1f;
        attackTime = -1f;
        chasingTime = 3f;

        //stunTime = 3;
        stunDeltaTime = -1;

        chasingDeltaTime = -1f;

        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        speed = 2;
        agent.speed = speed;

        attackSpeed = 1f;
        healthPoints = 20;

        secondaryState = SecondaryState.None;

        state = State.Roam;
        Vector3 dest = GetRoamPosition();
        agent.destination = dest;
    }

    void Update()
    {
        switch (secondaryState)
        {
            case SecondaryState.Stun:
                if (stunDeltaTime > 0f)
                {

                }

                break;
            case SecondaryState.Root:
                break;
            case SecondaryState.Slow:
                break;
            default: break;
        }

        switch (state)
        {
            case State.Roam:
                Roam();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Idle:
                Idle();
                break;
            default:
                break;
        }

    }

    private void Roam()
    {
        if (IsPlayerInRange(maxRangeToPlayer))
        {
            state = State.Chase;
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            state = State.Idle;
            agent.isStopped = true;
        }
    }

    private void Chase()
    {
        if (!IsPlayerInRange(maxRangeToPlayer))
        {
            if (chasingDeltaTime > 0 && playerController)
            {
                agent.SetDestination(playerController.transform.position);
                chasingDeltaTime -= Time.deltaTime;
                return;
            }
            state = State.Roam;
            agent.isStopped = false;
            agent.SetDestination(GetRoamPosition());
            return;
        }
        else
        {
            attackTime -= Time.deltaTime;
            if (!IsPlayerInRange(attackRange))
            {
                agent.isStopped = false;
                agent.SetDestination(playerController.transform.position);
            }
            else
            {
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

    private void Idle()
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
            agent.SetDestination(GetRoamPosition());
            idleTime = Random.Range(1f, 3f);
        }
    }

    public abstract void Attack();

    public abstract bool Stun();
    public abstract bool Slow();
    public abstract bool Root();

    private bool IsPlayerInRange(float range)
    {
        if (!playerController)
        {
            return false;
        }
        return Vector3.Distance(playerController.transform.position, transform.position) < range;
    }

    private Vector3 GetRoamPosition()
    {
        return startingPosition + RandomUtils.GetRandomDirection() * Random.Range(moveRangeMin, moveRangeMax);
    }

    public void ReceiveDamage(float amount)
    {
        agent.isStopped = false;
        state = State.Chase;
        chasingDeltaTime = chasingTime;
        if ((healthPoints -= amount) <= 0) Destroy(gameObject);
    }

    public EObjectType GetObjectType()
    {
        return EObjectType.Enemy;
    }
}
