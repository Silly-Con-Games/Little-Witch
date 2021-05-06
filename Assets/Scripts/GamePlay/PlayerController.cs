using UnityEngine;
using Config;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamagable
{
    [Tooltip("If false its relative to camera")]
    public bool movementRelativeToWitch = false;
    public CharacterController characterController;

    public ChargeAbility chargeAbility;
    public MeleeAbility meeleeAbility;

    private Camera mainCamera;
    private Transform cameraTrans;

    private Animator animator;

    private Health health;

    const float gravity = -9.81f;
    float upVelocity = 0;
    Vector2 inputVelocity;

    float speed = 3;
    float speedModifier = 1;
    float jumpHeight = 1.0f;
    bool wantsJump;

    private void Start()
    {
        mainCamera = Camera.main;
        cameraTrans = mainCamera.transform;
        animator = GetComponent<Animator>();
        GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);
        ApplyConfig();
    }

    private void OnDestroy()
    {
        GlobalConfigManager.onConfigChanged.RemoveListener(ApplyConfig);
    }

    void Update()
    {
        MoveUpdate();

        if(chargeAbility.IsCharging)
            chargeAbility.UpdateAnimation();
    }

    void ApplyConfig()
    {
        var witchConfig = GlobalConfigManager.GetWitchConfig();

        speed = witchConfig.movementSpeed;
        jumpHeight = witchConfig.jumpHeight;
        health = new Health(witchConfig.health);
        meeleeAbility.conf = witchConfig.meeleeAbility;
        chargeAbility.conf = witchConfig.chargeAbility;
    }

    void MoveUpdate()
    {
        // Direction
        Ray ray = mainCamera.ScreenPointToRay(Pointer.current.position.ReadValue());

        RaycastHit hit;
        bool didHit = false;
        if (didHit = Physics.Raycast(ray, out hit, 1000))
        {
            var targetPosition = hit.point;
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

<<<<<<< Updated upstream
=======
    void CheckCurrentBiome()
    {
        BiomeType newType = mapController.BiomeTypeInPosition(transform.position);
        if(newType != standingOnBiomeType)
        {
            switch (standingOnBiomeType)
            {
                case BiomeType.meadow: // Remove meadow passive effect
                    meadowAbility.SteppedFromMeadow();
                    break;
                case BiomeType.water: // Remove water passive effect
                    waterAbility.SteppedFromWater();
                    break;
            }

            standingOnBiomeType = newType;

            hudController.UpdateAbilityIcons(standingOnBiomeType);

            switch (standingOnBiomeType)
            {
                case BiomeType.forest:
                    currentMainAbility = forestAbility;
                    break;
                case BiomeType.meadow:
                    currentMainAbility = meadowAbility;
                    meadowAbility.SteppedOnMeadow();
                    break;
                case BiomeType.water:
                    currentMainAbility = waterAbility;
                    waterAbility.SteppedOnWater();
                    break;
                default:
                    currentMainAbility = null;
                    break;
            }

            if (standingOnBiomeType == BiomeType.unknown)
                Debug.LogWarning("Stepped on unknown biome type at position " + transform.position);

        }
    }

>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
=======
    public void OnMainAbility(InputValue value)
    {
        if(currentMainAbility != null && currentMainAbility.IsReady)
        {
            currentMainAbility.CastAbility();
            hudController.StartAbilityCooldown(currentMainAbility);
        }
        else
        {
            Debug.Log("Unable to cast ability on " + standingOnBiomeType);
            if(currentMainAbility != null) hudController.AbilityNotReady(currentMainAbility);
        }
    }

>>>>>>> Stashed changes
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

    private class Health
    {
        private float maxHealth = 0;
        private float health = 0;
        public Health(float maxHealth) => health = this.maxHealth = maxHealth;
        public bool IsDepleted => health <= 0;
        public void ResetHealth() => health = maxHealth;
        public void TakeDamage(float amount) => health = Mathf.Max(0, health - amount);
        public void Heal(float amount) => health = Mathf.Min(maxHealth, health + amount);
    }
}
