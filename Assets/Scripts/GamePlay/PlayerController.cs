using Assets.Scripts.GameEvents;
using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerController : MonoBehaviour, IDamagable
{
    [Tooltip("If false its relative to camera")]
    public bool movementRelativeToWitch = false;
    public CharacterController characterController;
    public MapController mapController;
    public HUDController hudController;
    public TransformMenu transformMenu;
    public PauseController pauseController;

    public UnityEvent onDeathEvent;

    public ChargeAbility chargeAbility;
    public MeleeAbility meleeAbility;

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

    private Vector2 inputRotation;

    private Vector3 lastPos;

    float speed = 3;
    float speedModifier = 1;
    float jumpHeight = 1.0f;
    int tileMask;
    bool wantsJump;

    private bool isDead;

    private float rotationSpeed = 1000f;

    public bool gamepadActive = false;

    public bool canBeControlled = true;

    public  Vector3 mouseWorldPosition { get; internal set; }

    [SerializeField] private AimingGfxController aimGfx;

    public void Initialize()
    {
        lastPos = transform.position;
        tileMask = LayerMask.GetMask("Tile");

        mainCamera = Camera.main;
        cameraTrans = mainCamera.transform;
        animator = GetComponentInChildren<Animator>();

        GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);

        passiveEffects = new HashSet<UnityAction>();

		transformAbility.Init(this);

        meleeAbility.player = this;        
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
        }

        SetUpInput();

        hudController.playerController = this;

        //aimGfx.playerController = this;

        ApplyConfig();

        isDead = false;
    }

    private void SetUpInput()
    {
        PlayerInput pi = GetComponent<PlayerInput>();

        if (!transformMenu)
        {
            transformMenu = FindObjectOfType<TransformMenu>();
            transformMenu.playerController = this;
            pi.actions["TransformMenu"].performed += _ => transformMenu.OpenMenu(transformAbility.IsReady());
            pi.actions["TransformMenu"].canceled += _ => transformMenu.CloseMenu();
        }

        pi.actions["MainAbility"].performed += _ => AimMainAbility();
        pi.actions["MainAbility"].canceled += _ => CastMainAbility();

        if (pi.uiInputModule == null) pi.uiInputModule = FindObjectOfType<InputSystemUIInputModule>();
    }

    void Update()
    {
        MoveUpdate();

        CheckCurrentBiome();

        if (chargeAbility.IsCharging)
            chargeAbility.UpdateAnimation();

        foreach(var passive in passiveEffects)
            passive();

		mapController.SetPlayerPosition(transform.position);

        if (aimGfx.isOn)
        {
            if (currentMainAbility == null) aimGfx.SetReady(false);
            else aimGfx.SetReady(currentMainAbility.IsReady);
        }

    }

    private void OnDestroy()
    {
        health.Cleanup();
        energy.Cleanup();
    }

    private void ApplyConfig()
    {
        var witchConfig = GlobalConfigManager.GetWitchConfig();

        speed = witchConfig.movementSpeed;
        jumpHeight = witchConfig.jumpHeight;

        health = new HealthTracker(witchConfig.health);
        hudController.SetUpHealth(health.Health, health.MaxHealth);
        health.onChanged.AddListener(hudController.SetHealth);
        hudController.SetHealth(health.Health);

        energy = new EnergyTracker(witchConfig.energyMax, witchConfig.energyInitial);
        energy.onChanged.AddListener(ChangeEnergyTankAppearance);
        ChangeEnergyTankAppearance(energy.Energy);

        hudController.SetUpEnergy(energy.Energy, energy.MaxEnergy, 1);//Mathf.CeilToInt(energy.MaxEnergy/witchConfig.transformAbility.energyCost));
        energy.onChanged.AddListener(hudController.SetEnergy);
        energy.onNotEnough.AddListener(hudController.NotEnoughEnergy);
        hudController.SetEnergy(energy.Energy);

        meleeAbility.conf = witchConfig.meeleeAbility;
        chargeAbility.conf = witchConfig.chargeAbility;

		transformAbility.conf = witchConfig.transformAbility;

        forestAbility.conf = witchConfig.forestAbility;
        waterAbility.conf = witchConfig.waterAbility;
        meadowAbility.conf = witchConfig.meadowAbility;
        dashAbility.conf = witchConfig.dashAbility;

        OnControlsChanged(GetComponent<PlayerInput>());
    }

    void MoveUpdate()
    {
        if (!canBeControlled) return;

        // Direction
        if (gamepadActive)
        {
            Vector3 lookDir = inputRotation == Vector2.zero ?
                new Vector3(inputVelocity.x, 0, inputVelocity.y).normalized :
                new Vector3(inputRotation.x, 0, inputRotation.y).normalized;
            mouseWorldPosition = transform.position + lookDir * forestAbility.conf.maxRange;  // used for abilities' direction computation
            if (lookDir == Vector3.zero) lookDir = Vector3.forward;
            Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
        else
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            float camDist = Vector3.Distance(transform.position, mainCamera.transform.position);
            Vector3 lookDir = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, camDist));
            mouseWorldPosition = lookDir;
            lookDir.y = transform.position.y;
            transform.LookAt(lookDir);
        }

        if (moveStop)
            return;

        // Movement sound
        Vector3 diff = transform.position - lastPos;
        if (diff.sqrMagnitude > stepLengthSqr)
        {
            lastPos = transform.position;

            FMODUnity.RuntimeManager.PlayOneShot("event:/test/step");
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

    public void OnDash(InputValue value)
    {
        if (!canBeControlled) return;

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
        if (!canBeControlled) return;

        if (!pauseController.IsPaused() && meleeAbility.IsReady)
        {
            meleeAbility.Attack();
            ScaleSpeedModifier(meleeAbility.conf.attackSlow);
            StartCoroutine(ResumeMovement(meleeAbility.conf.cooldown));
        }
        else if (!pauseController.IsPaused())
        {
            GameEventQueue.QueueEvent(new MeleeAbilityEvent(failedCast : true));
        }
    }

    IEnumerator ResumeMovement(float duration)
    {
        yield return new WaitForSeconds(duration);
        ScaleSpeedModifier(1.0f/meleeAbility.conf.attackSlow);        
    }

    public void OnChargeAbility(InputValue value)
    {
        if (!canBeControlled) return;

        if (!pauseController.IsPaused()) {
            if (value.isPressed && chargeAbility.IsReady())
            {
                animator.SetTrigger("Cast");

                chargeAbility.StartCharge();
            }
            else if (!value.isPressed && chargeAbility.IsCharging)
            {
                chargeAbility.FireCharged();
            }
            else if (!chargeAbility.IsReady())
                GameEventQueue.QueueEvent(new ChargeAbilityEvent(failCast:true));
		}
    }

    public void AimMainAbility()
    {
        if (!canBeControlled) return;

        if (currentMainAbility != null)
        {
            aimGfx.Show(ready: currentMainAbility.IsReady);
            if (!currentMainAbility.IsReady)
            {
                hudController.AbilityNotReady(currentMainAbility);
                GameEventQueue.QueueEvent(new MainAbilityFailEvent(notOnCd: true));
            }
        }
        else
        {
            GameEventQueue.QueueEvent(new MainAbilityFailEvent(deadBiome: true));
        }
    }

    public void CastMainAbility()
    {
        if (!canBeControlled) return;

        aimGfx.Hide();

        if (currentMainAbility != null && currentMainAbility.IsReady)
        {
            animator.SetTrigger("Cast");

            currentMainAbility.CastAbility();
            if (currentMainAbility is MeadowAbility)
            {
                StartCoroutine(PlayMeadowPathSoundCoroutine());
            }
            hudController.CastAbility(currentMainAbility);
        }
/*        else if (currentMainAbility != null)
        {
            hudController.AbilityNotReady(currentMainAbility);
            GameEventQueue.QueueEvent(new MainAbilityFailEvent(notOnCd: true));
        }
        else
        {
            GameEventQueue.QueueEvent(new MainAbilityFailEvent(deadBiome: true));
        }*/

    }

    IEnumerator PlayMeadowPathSoundCoroutine()
    {
        yield return new WaitForSeconds(0.4f);
         
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
        if (!canBeControlled) return;

        if (transformAbility.IsReady())
        {
            transformAbility.Revive();
        }
        else
            GameEventQueue.QueueEvent(new BiomeTransformationFailedEvent(noEnergy: true, revive: true));
    }

	public void Transform(BiomeType target) {
        if (!canBeControlled) return;

        if (transformAbility.IsReady())
        {
            transformAbility.Transform(target);
        }
        else
            GameEventQueue.QueueEvent(new BiomeTransformationFailedEvent(noEnergy: true));
    }

    private void OnPause(InputValue value) {
		if (pauseController.IsPaused()) {
			pauseController.ResumeGame();
		} else {
			pauseController.PauseGame();
		}
	}

    private void OnLook(InputValue value)
    {
        // only used with controller input
        inputRotation = value.Get<Vector2>();
    }

    private void OnTransformMenu(InputValue value)
    {

    }

    public void ReceiveDamage(float amount)
    {
        if (isDead) return;
        FMODUnity.RuntimeManager.PlayOneShot("event:/witch/hit/witch_hit");
        FMODUnity.RuntimeManager.PlayOneShot("event:/witch/hit/witch_ouch");
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
        GameEventQueue.QueueEvent(new PlayerDeathEvent());
		this.enabled = false;
        GetComponent<PlayerInput>().enabled = false;
    }

    public EObjectType GetObjectType() => EObjectType.Player;

    public void ScaleSpeedModifier(float val)
    {
        speedModifier *= val;
    }

    public void ChangeEnergyTankAppearance(float curEnergy)
    {
        //Debug.Log("changing energy tank appearance");
        animator.SetBool("EnoughEnergy", curEnergy >= transformAbility.conf.energyCost);
    }

    public void OnControlsChanged(PlayerInput pi)
    {
        Debug.Log("controls changed");
        gamepadActive = pi.currentControlScheme.Equals("Gamepad");
        hudController.SwitchControlScheme(pi);
    }

}
