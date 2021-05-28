using UnityEngine;
using Config;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Collections;

public class PlayerController : MonoBehaviour, IDamagable
{
    [Tooltip("If false its relative to camera")]
    public bool movementRelativeToWitch = false;
    public CharacterController characterController;
    public MapController mapController;
    public HUDController hudController;
	public PauseController pauseController;

    public UnityEvent onDeathEvent;

    public ChargeAbility chargeAbility;
    public MeleeAbility meeleeAbility;

	public TransformAbility transformAbility;

    public ForestAbility forestAbility;
    public MeadowAbility meadowAbility;
    public WaterAbility waterAbility;
    public DashAbility dashAbility;

    public HashSet<UnityAction> passiveEffects { get; internal set; }

    private Camera mainCamera;
    private Transform cameraTrans;
    private Animator animator;

    public HealthTracker health { get; internal set; }
    public EnergyTracker energy { get; internal set; }

	private MainAbility currentMainAbility;
    private BiomeType standingOnBiomeType = BiomeType.UNKNOWN;

    const float gravity = -9.81f;
    float upVelocity = 0;

    public Vector2 inputVelocity;
    public bool moveStop { get => moveStopInternal; set { moveStopInternal = value; lastPos = transform.position; } }
    private bool moveStopInternal;
    public float stepLengthSqr = 1.78f;

    private Vector3 lastPos;

    float speed = 3;
    float speedModifier = 1;
    float jumpHeight = 1.0f;
    int tileMask;
    bool wantsJump;

    private bool isDead;

    public  Vector3 mouseWorldPosition { get; internal set; }

    private void Start()
    {
        lastPos = transform.position;
        tileMask = LayerMask.GetMask("Tile");

        mainCamera = Camera.main;
        cameraTrans = mainCamera.transform;
        animator = GetComponentInChildren<Animator>();

        GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);

        passiveEffects = new HashSet<UnityAction>();

		transformAbility.Init(this);

        meeleeAbility.player = this;        
        forestAbility.Init(this);
        meadowAbility.Init(this);
        waterAbility.Init(this);
        dashAbility.Init(this);

        if (!mapController)
            mapController = FindObjectOfType<MapController>();

        if (!hudController)
        {
            hudController = FindObjectOfType<HUDController>();
            hudController.playerController = this;
            Debug.Log("setting hudcont player cont to " + this);
        }

        hudController.playerController = this;

        ApplyConfig();

        isDead = false;
    }

    void Update()
    {
        MoveUpdate();

        CheckCurrentBiome();

        if (meeleeAbility.attackInQ)
        {
            meeleeAbility.attackInQ = false;
            OnMeleeAbility(null);
        }

        if (chargeAbility.IsCharging)
            chargeAbility.UpdateAnimation();

        foreach(var passive in passiveEffects)
            passive();

    }

    void ApplyConfig()
    {
        var witchConfig = GlobalConfigManager.GetWitchConfig();

        speed = witchConfig.movementSpeed;
        jumpHeight = witchConfig.jumpHeight;

        health = new HealthTracker(witchConfig.health);
        hudController.SetUpHealth(health.Health, health.MaxHealth);
        health.onChanged.AddListener(hudController.SetHealth);

        energy = new EnergyTracker(witchConfig.energyMax, witchConfig.energyInitial);
        
        hudController.SetUpEnergy(energy.Energy, energy.MaxEnergy, Mathf.CeilToInt(energy.MaxEnergy/witchConfig.transformAbility.energyCost));
        energy.onChanged.AddListener(hudController.SetEnergy);
        energy.onNotEnough.AddListener(hudController.NotEnoughEnergy);

        meeleeAbility.conf = witchConfig.meeleeAbility;
        chargeAbility.conf = witchConfig.chargeAbility;

		transformAbility.conf = witchConfig.transformAbility;

        forestAbility.conf = witchConfig.forestAbility;
        waterAbility.conf = witchConfig.waterAbility;
        meadowAbility.conf = witchConfig.meadowAbility;
        dashAbility.conf = witchConfig.dashAbility;
    }

    void MoveUpdate()
    {
        // Direction
        Ray ray = mainCamera.ScreenPointToRay(Pointer.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000, tileMask))
        {
            mouseWorldPosition = hit.point;
        }

        if (moveStop)
            return;

        Vector3 diff = transform.position - lastPos;
        if (diff.sqrMagnitude > stepLengthSqr)
        {
            lastPos = transform.position;

            FMODUnity.RuntimeManager.PlayOneShot("event:/test/step");
        }

        var targetPosition = mouseWorldPosition;
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);

        // Movement
        Vector3 forwardV = cameraTrans.forward;
        forwardV.y = 0;
        forwardV.Normalize();
        Vector3 rightV = cameraTrans.right;

        Vector3 velocity;
        if(!movementRelativeToWitch)
            velocity = inputVelocity.y * forwardV + inputVelocity.x * rightV ;
        else
            velocity = inputVelocity.y * transform.forward + inputVelocity.x * transform.right ; // relative to witch rotation

        float delta = Time.deltaTime;

        velocity.Normalize();
        velocity *= speed * speedModifier;

        if (characterController.isGrounded)
        {
            upVelocity = 0;
            if (wantsJump)
                upVelocity += Mathf.Sqrt(jumpHeight * -3f * gravity);
        }
        else
            upVelocity += gravity * delta;
        wantsJump = false;

        velocity.y = upVelocity;

        float velocityX = Vector3.Dot(velocity.normalized, transform.forward);
        float velocityZ = Vector3.Dot(velocity.normalized, transform.right);

        animator.SetFloat("VelocityX", velocityX, .1f, Time.deltaTime);
        animator.SetFloat("VelocityZ", velocityZ, .1f, Time.deltaTime);
        if (velocity.magnitude > 0)
            characterController.Move(velocity * delta);
    }

    void CheckCurrentBiome()
    {
        BiomeType newType = mapController.BiomeTypeInPosition(transform.position);
        if(newType != standingOnBiomeType)
        {
            switch (standingOnBiomeType)
            {
                case BiomeType.MEADOW: // Remove meadow passive effect
                    meadowAbility.SteppedFromMeadow();
                    break;
                case BiomeType.WATER: // Remove water passive effect
                    waterAbility.SteppedFromWater();
                    break;
            }

            standingOnBiomeType = newType;

            hudController.UpdateAbilityIcons(standingOnBiomeType);

            switch (standingOnBiomeType)
            {
                case BiomeType.FOREST:
                    currentMainAbility = forestAbility;
                    break;
                case BiomeType.MEADOW:
                    currentMainAbility = meadowAbility;
                    meadowAbility.SteppedOnMeadow();
                    break;
                case BiomeType.WATER:
                    currentMainAbility = waterAbility;
                    waterAbility.SteppedOnWater();
                    break;
                default:
                    currentMainAbility = null;
                    break;
            }

            if (standingOnBiomeType == BiomeType.UNKNOWN)
                Debug.LogWarning("Stepped on unknown biome type at position " + transform.position);

        }
    }

    public void OnMove(InputValue value)
    {
        inputVelocity = value.Get<Vector2>();
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed && dashAbility.IsReady)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/witch/dash/dash");
            dashAbility.CastAbility();
        }
           
    }

    public void OnJummp(InputValue value)
    {
        if (value.isPressed)
        {
            wantsJump = true;
        }
    }

    public void OnMeleeAbility(InputValue value)
    {
        if (!pauseController.IsPaused() && meeleeAbility.IsReady)
        {
            meeleeAbility.Attack();
            ScaleSpeedModifier(meeleeAbility.conf.attackSlow);
            StartCoroutine(ResumeMovement(meeleeAbility.conf.cooldown));
        }
        else if (!pauseController.IsPaused())
        {
            meeleeAbility.attackInQ = true;
        }
    }

    IEnumerator ResumeMovement(float duration)
    {
        yield return new WaitForSeconds(duration);
        ScaleSpeedModifier(1.0f/meeleeAbility.conf.attackSlow);        
    }

    public void OnChargeAbility(InputValue value)
    {
        if (!pauseController.IsPaused()) {
			if (value.isPressed && chargeAbility.IsReady()) {
				animator.SetTrigger("Cast");

				chargeAbility.StartCharge();
			} else if (!value.isPressed && chargeAbility.IsCharging) {
				chargeAbility.FireCharged();
			}
		}
    }

    public void OnMainAbility(InputValue value)
    {
        if(currentMainAbility != null && currentMainAbility.IsReady)
        {
            animator.SetTrigger("Cast");

            currentMainAbility.CastAbility();
            hudController.CastAbility(currentMainAbility);
        }
        else if (currentMainAbility != null)
        {
            hudController.AbilityNotReady(currentMainAbility);
        }

    }

	public void OnTransformForest(InputValue value) {
		Transform(BiomeType.FOREST);
	}

	public void OnTransformMeadow(InputValue value) {
		Transform(BiomeType.MEADOW);
	}

	public void OnTransformWater(InputValue value) {
		Transform(BiomeType.WATER);
	}

	public void OnTransformDead(InputValue value) {
		Transform(BiomeType.DEAD);
	}

    public void OnRevive(InputValue value)
    {
        if (transformAbility.IsReady())
        {
            transformAbility.Revive();
        }
    }

	private void Transform(BiomeType target) {
		if (transformAbility.IsReady()) {
			transformAbility.Transform(target);
		}
	}

	private void OnPause(InputValue value) {
		if (pauseController.IsPaused()) {
			pauseController.ResumeGame();
		} else {
			pauseController.PauseGame();
		}
	}

	public void ReceiveDamage(float amount)
    {
        if (isDead) return;
        FMODUnity.RuntimeManager.PlayOneShot("event:/witch/hit/witch_hit");
        animator.SetTrigger("GetHit");

        health.TakeDamage(amount);
        if (health.IsDepleted) Die();
    }

    public void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

		GlobalConfigManager.onConfigChanged.RemoveListener(ApplyConfig);
		onDeathEvent.Invoke();

		this.enabled = false;
        GetComponent<PlayerInput>().enabled = false;
    }

    public EObjectType GetObjectType() => EObjectType.Player;

    public void ScaleSpeedModifier(float val)
    {
        speedModifier *= val;
    }
}
