using UnityEngine;
using Config;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;

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

    public ForestAbility forestAbility;
    public MeadowAbility meadowAbility;
    public WaterAbility waterAbility;
    

    private Camera mainCamera;
    private Transform cameraTrans;
    private Animator animator;

    public HealthTracker health { get; internal set; }
    public EnergyTracker energy { get; internal set; }

    private MainAbility currentMainAbility;
    private BiomeType standingOnBiomeType = BiomeType.unknown;

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
        animator = GetComponent<Animator>();
        GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);

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

        if(chargeAbility.IsCharging)
            chargeAbility.UpdateAnimation();

        CheckCurrentBiome();
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

        meeleeAbility.conf = witchConfig.meeleeAbility;
        chargeAbility.conf = witchConfig.chargeAbility;
        forestAbility.conf = witchConfig.forestAbility;
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
            standingOnBiomeType = newType;
            switch (standingOnBiomeType)
            {
                case BiomeType.forest:
                    currentMainAbility = forestAbility;
                    break;
                case BiomeType.meadow:
                    currentMainAbility = meadowAbility;
                    break;
                case BiomeType.water:
                    currentMainAbility = waterAbility;
                    break;
                default:
                    currentMainAbility = null;
                    break;
            }

            if (standingOnBiomeType == BiomeType.unknown)
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
        }
        else
        {
            Debug.Log("Unable to cast ability on " + standingOnBiomeType);
        }
    }

    public void ReceiveDamage(float amount)
    {
        health.TakeDamage(amount);
        if (health.IsDepleted) Destroy(gameObject);
    }

    public EObjectType GetObjectType() => EObjectType.Player;

    public void SetSpeedModifier(float val)
    {
        speedModifier = val;
    }
}
