using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public abstract class EnemyAI : MonoBehaviour, IDamagable
{
    public enum State
    {
        Roam,
        Chase,
        Idle,
        Attack
    }

    public enum SecondaryState
    {
        Stun,
        Root,
        None
    }

    public State state { get; set; }
    public SecondaryState secondaryState { get; set; }

    //[SerializeField]
    //private float stunTime;

    protected float stunDeltaTime;

    [SerializeField]
    protected float idleTime;

    [SerializeField]
    protected float attackTime;

    [SerializeField]
    protected float chasingTime;

    protected float chasingDeltaTime;

    protected NavMeshAgent agent;

    protected Transform roamPosition;

    [SerializeField]
    protected PlayerController playerController;

    [SerializeField]
    protected float moveRangeMin;

    [SerializeField]
    protected float moveRangeMax;

    [SerializeField]
    protected float maxRangeToPlayer;

    [SerializeField]
    protected float attackRange;

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float attackSpeed;

    [SerializeField]
    protected float healthPoints;

    [SerializeField]
    protected float slowDefault;

    [SerializeField]
    protected float rootDefault;

    public virtual void InitEnemy()
    {
        if (!playerController)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
        roamPosition = this.transform;
        moveRangeMin = 4f;
        moveRangeMax = 4f;
        maxRangeToPlayer = 8f;

        idleTime = -1f;
        attackTime = -1f;
        chasingTime = 3f;
        slowDefault = 3f;
        rootDefault = 3f;

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
    }

    public virtual void InitEnemy(Transform roamPosition)
    {
        InitEnemy();
        this.roamPosition = roamPosition;
    }

    void Update()
    {
        if (secondaryState == SecondaryState.Stun)
        {
            return;
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
            case State.Attack:
                Attack();
                break;
            default:
                break;
        }
    }

    protected abstract void Roam();
    protected abstract void Chase();
    protected abstract void Idle();
    protected abstract void Attack();

    public virtual void Stun(float stunDuration)
    {
        if (!(secondaryState == SecondaryState.Stun))
        {
            secondaryState = SecondaryState.Stun;
            StartCoroutine(RootCoroutine(stunDuration));
        }
    }

    public virtual void Root(float rootDuration)
    {
        secondaryState = SecondaryState.Root;
        StartCoroutine(RootCoroutine(rootDuration));
    }

    protected IEnumerator RootCoroutine(float rootDuration)
    {
        agent.speed = 0;
        while (rootDuration >= 0)
        {
            rootDuration -= Time.deltaTime;
            yield return null;
        }
        agent.speed = speed;
        secondaryState = SecondaryState.None;
    }

    public virtual void Slow(float slowDuration)
    {
        StartCoroutine(SlowCoroutine(slowDuration));
    }

    protected IEnumerator SlowCoroutine(float slowDuration)
    {
        agent.speed = speed / 2f;
        while (slowDuration >= 0)
        {
            slowDuration -= Time.deltaTime;
            yield return null;
        }

        agent.speed = speed;
    }

    protected bool IsPlayerInRange(float range)
    {
        if (!playerController)
        {
            return false;
        }
        return Vector3.Distance(playerController.transform.position, transform.position) < range;
    }

    public virtual void ReceiveDamage(float amount)
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

    public void SetRoamObjectTransform(Transform transform)
    {
        this.roamPosition = transform;
    }
    
}
