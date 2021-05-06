using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public abstract class EnemyAI : MonoBehaviour, IDamagable, IRootable, IStunnable, ISlowable
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
    protected float idleDuration;

    protected float attackCooldownDelta;

    [SerializeField]
    protected float chasingDuration;

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
    protected float attackCooldown;

    [SerializeField]
    protected float healthPoints;

    [SerializeField]
    protected float slowDefault;

    [SerializeField]
    protected float rootDefault;

    [SerializeField]
    private GameObject energyPrefab;

    protected Animator animator = null;
    public virtual void InitEnemy()
    {
        GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);
        if (!playerController)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
        roamPosition = this.transform;
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        secondaryState = SecondaryState.None;
        state = State.Roam;
        if (!animator)
        {
            animator = GetComponentInChildren<Animator>();
        }
        ApplyConfig();
    }

    protected virtual void ApplyConfig()
    {
        var enemyConfig = GetEnemyBaseConfig();
        roamPosition = this.transform;
        moveRangeMin = enemyConfig.moveRangeMin;
        moveRangeMax = enemyConfig.moveRangeMax;
        attackRange = enemyConfig.attackRange;
        maxRangeToPlayer = enemyConfig.maxRangeToPlayer;
        idleDuration = enemyConfig.idleDuration;
        chasingDuration = enemyConfig.chasingDuration;
        slowDefault = enemyConfig.slowDefault;
        rootDefault = enemyConfig.rootDefault;
        stunDeltaTime = enemyConfig.rootDefault;
        chasingDeltaTime = enemyConfig.chasingDuration;
        speed = enemyConfig.speed;
        agent.speed = speed;
        attackCooldown = enemyConfig.attackCooldown;
        healthPoints = enemyConfig.healthPoints;
    }

    protected abstract EnemyConfig GetEnemyBaseConfig();

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
        animator.SetTrigger("GetHit");
        FMODUnity.RuntimeManager.PlayOneShot("event:/enemies/hit/generic_hit");
        agent.isStopped = false;
        state = State.Chase;
        chasingDeltaTime = chasingDuration;
        Debug.Log(healthPoints);
        if ((healthPoints -= amount) <= 0) Die();
        Stun(5);
    }

    public virtual bool IsCloseToAttack()
    {
        if (!playerController)
        {
            return false;
        }
        return Vector3.Distance(transform.position, playerController.transform.position) <= attackRange;
    }

    public EObjectType GetObjectType()
    {
        return EObjectType.Enemy;
    }

    public void RecieveSlow(float duration)
    {
        Slow(duration );
    }

    public void RecieveStun(float duration)
    {
        Stun(duration);
    }

    public void ReceiveRoot(float duration)
    {
        Root(duration);
    }

    public void SetRoamObjectTransform(Transform transform)
    {
        this.roamPosition = transform;
    }

    protected void Die()
    {
        GameObject energy = Instantiate(energyPrefab);
        energy.transform.position = new Vector3(this.transform.position.x, 0.5f, this.transform.position.z);
        GlobalConfigManager.onConfigChanged.RemoveListener(ApplyConfig);
        Destroy(gameObject);
    }
    
}
