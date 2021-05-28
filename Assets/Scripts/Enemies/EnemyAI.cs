using System.Collections;
using Config;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyAI : MonoBehaviour, IDamagable, IRootable, IStunnable, ISlowable, IPushable
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

    protected float idleDeltaTime;

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
    protected float damage;
    
    [SerializeField]
    private GameObject energyPrefab;

    public GameObject indicator { get; set; }

    protected Animator animator = null;
    public virtual void InitEnemy()
    {
        EnemiesController.IncreaseAliveCount();
        GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);
        if (!playerController)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
        roamPosition = null;
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        secondaryState = SecondaryState.None;
        state = State.Roam;
        if (!animator)
        {
            animator = GetComponentInChildren<Animator>();
        }

        this.indicator = IndicatorsCreator.CreateIndicator();
        this.indicator.SetActive(false);
        ApplyConfig();
    }

    protected virtual void ApplyConfig()
    {
        var enemyConfig = GetEnemyBaseConfig();
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
        damage = enemyConfig.damage;
    }

    protected abstract EnemyConfig GetEnemyBaseConfig();

    public virtual void InitEnemy(Transform roamPosition)
    {
        InitEnemy();
        this.roamPosition = roamPosition;
    }

    protected void Update()
    {
        chasingDeltaTime -= Time.deltaTime;
        attackCooldownDelta -= Time.deltaTime;

        UpdateIndicator();
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

        float speed = agent.velocity.sqrMagnitude;
        animator.SetFloat("Speed", speed);
    }
    private void UpdateIndicator()
    {
        
        if (!playerController)
            return;

        if (!indicator)
            return;
        
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        
        
        if (pos.x > 0 && pos.x < Screen.width && pos.y > 0 && pos.y < Screen.height)
        {
            indicator.SetActive(false);
        }
        else
        {
            indicator.SetActive(true);
        }
        
        Vector2 posMy = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 posPlayer = Camera.main.WorldToScreenPoint(playerController.transform.position);

        Vector2 intersection;

        if (LineUtils.GetIntersectWithScreenEdges(posMy, posPlayer, out intersection))
        {
            indicator.GetComponent<RectTransform>().position = new Vector3(intersection.x, intersection.y, 0);
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
        return Vector3.Distance(playerController.transform.position, transform.position) <= range;
    }

    public virtual void ReceiveDamage(float amount)
    {
        animator.SetTrigger("GetHit");
        agent.isStopped = false;
        state = State.Chase;
        chasingDeltaTime = chasingDuration;
        if ((healthPoints -= amount) <= 0) Die();
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
        EnemiesController.DecreaseAliveCount();
        GameObject energy = Instantiate(energyPrefab);
        energy.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        GlobalConfigManager.onConfigChanged.RemoveListener(ApplyConfig);
        Destroy(indicator);

        agent.enabled = false;
        GetComponent<Collider>().enabled = false;

        gameObject.layer = 0;
        foreach (Transform trans in gameObject.GetComponentsInChildren<Transform>())
        {
            trans.gameObject.layer = 0;
        }

        StartCoroutine(DieCoroutine());
    }

    protected IEnumerator DieCoroutine()
    {
        float duration = 3;
        float time = 0;
        float deathSpeed = 2;
        while ((time += Time.deltaTime) < duration)
        {
            transform.position += Vector3.down * Time.deltaTime * deathSpeed;
            yield return null;
        }
        Destroy(gameObject);
    }

    public void ReceivePush(Vector3 force, float duration)
    {
        StartCoroutine(PushCoroutine(force, duration));
    }

    IEnumerator PushCoroutine(Vector3 force, float duration)
    {
        float start = Time.time;
        while (Time.time - start <= duration)
        {
            agent.velocity = force;
            yield return null;
        }
        agent.velocity = Vector3.zero;
    }
}
