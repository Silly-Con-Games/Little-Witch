using UnityEngine;
using Config;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour, IDamagable
{
    [Tooltip("If false its relative to camera")]
    public bool movementRelativeToWitch = false;
    public CharacterController characterController;
    public MapController mapController;
    public HUDController hudController;

    public UnityEvent onDeathEvent;

    public ChargeAbility chargeAbility;
    public MeleeAbility meeleeAbility;

	public TransformAbility transformAbility;

    public ForestAbility forestAbility;
    public MeadowAbility meadowAbility;
    public WaterAbility waterAbility;

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
    Vector2 inputVelocity;

    float speed = 3;
    float speedModifier = 1;
    float jumpHeight = 1.0f;
    bool wantsJump;

    public  Vector3 mouseWorldPosition { get; internal set; }

    private void Start()
    {
        mainCamera = Camera.main;
        cameraTrans = mainCamera.transform;
        animator = GetComponentInChildren<Animator>();

        GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);

        passiveEffects = new HashSet<UnityAction>();

		transformAbility.Init(this);

        forestAbility.Init(this);
        meadowAbility.Init(this);
        waterAbility.Init(this);

        if (!mapController)
            mapController = FindObjectOfType<MapController>();

        if (!hudController)
            hudController = FindObjectOfType<HUDController>();

        ApplyConfig();
    }

    private void OnDestroy()
    {
        GlobalConfigManager.onConfigChanged.RemoveListener(ApplyConfig);
        onDeathEvent.Invoke();
    }

    void Update()
    {
        MoveUpdate();

        CheckCurrentBiome();

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
        hudController.SetUpEnergy(energy.Energy, energy.MaxEnergy);
        energy.onChanged.AddListener(hudController.SetEnergy);
        energy.onNotEnough.AddListener(hudController.NotEnoughEnergy);

        meeleeAbility.conf = witchConfig.meeleeAbility;
        chargeAbility.conf = witchConfig.chargeAbility;

		transformAbility.conf = witchConfig.transformAbility;

        forestAbility.conf = witchConfig.forestAbility;
        waterAbility.conf = witchConfig.waterAbility;
        meadowAbility.conf = witchConfig.meadowAbility;
    }

    void MoveUpdate()
    {
        // Direction
        Ray ray = mainCamera.ScreenPointToRay(Pointer.current.position.ReadValue());

        RaycastHit hit;
        bool didHit = false;
        if (didHit = Physics.Raycast(ray, out hit, 1000))
        {
            var targetPosition = mouseWorldPosition = hit.point;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);

        }

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

    public void OnJump(InputValue value)
    {
        if(value.isPressed)
            wantsJump = true;
    }

    public void OnMeleeAbility(InputValue value)
    {
        if(meeleeAbility.IsReady)
            meeleeAbility.Attack();
    }

    public void OnChargeAbility(InputValue value)
    {
        if (value.isPressed && chargeAbility.IsReady())
            chargeAbility.StartCharge();
        else if (!value.isPressed && chargeAbility.IsCharging)
            chargeAbility.FireCharged();
    }

    public void OnMainAbility(InputValue value)
    {
        if(currentMainAbility != null && currentMainAbility.IsReady)
        {
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

	private void Transform(BiomeType target) {
		if (transformAbility.IsReady()) {
			transformAbility.Transform(target);
		}
	}

	public void ReceiveDamage(float amount)
    {
        animator.ResetTrigger("Swing");
        animator.SetTrigger("GetHit");

        health.TakeDamage(amount);
        if (health.IsDepleted) Destroy(gameObject);
    }

    public EObjectType GetObjectType() => EObjectType.Player;

    public void ScaleSpeedModifier(float val)
    {
        speedModifier *= val;
    }
}
