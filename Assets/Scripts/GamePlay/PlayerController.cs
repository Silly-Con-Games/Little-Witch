using System;
using UnityEngine;
using Config;

public class PlayerController : MonoBehaviour, IDamagable
{
    public CharacterController characterController;
    public GameObject bulletPrefab;

    float speed = 3;
    float jumpHeight = 1.0f;

    private const float gravity = -9.81f;
    private float upVelocity = 0;

    private Camera mainCamera;
    private Transform cameraTrans;

    private SecondaryAbility secondaryAbility;
    private Health health;

    private void Start()
    {
        mainCamera = Camera.main;
        cameraTrans = mainCamera.transform;
        GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);
        ApplyConfig();
    }

    private void OnDestroy()
    {
        GlobalConfigManager.onConfigChanged.RemoveListener(ApplyConfig);
    }


    // Update is called once per frame
    void Update()
    {
        MoveUpdate();

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        bool didHit = false;
        if (didHit = Physics.Raycast(ray, out hit, 1000))
        {
            var targetPosition = hit.point;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
        }

        secondaryAbility.Update(ref hit, didHit);
    }

    void ApplyConfig()
    {
        var witchConfig = GlobalConfigManager.GetWitchConfig();

        speed = witchConfig.movementSpeed;
        jumpHeight = witchConfig.jumpHeight;
        health = new Health(witchConfig.health);
        secondaryAbility = new SecondaryAbility(ref witchConfig.secondaryAbility, bulletPrefab, transform);
    }

    void MoveUpdate()
    {
        Vector3 forwardV = cameraTrans.forward;
        forwardV.y = 0;
        forwardV.Normalize();
        Vector3 rightV = cameraTrans.right;
        Vector3 velocity = Vector3.zero;
        float delta = Time.deltaTime;

        if (Input.GetKey(KeyCode.D))
            velocity += rightV;
        if (Input.GetKey(KeyCode.A))
            velocity -= rightV;

        if (Input.GetKey(KeyCode.W))
            velocity += forwardV;
        if (Input.GetKey(KeyCode.S))
            velocity -= forwardV;

        velocity.Normalize();
        velocity *= speed;

        if (characterController.isGrounded)
        {
            upVelocity = 0;
            if (Input.GetKey(KeyCode.Space))
                upVelocity += Mathf.Sqrt(jumpHeight * -3f * gravity);
        }
        else
            upVelocity += gravity * delta;

        velocity.y = upVelocity;

        if (velocity.magnitude > 0)
            characterController.Move(velocity * delta);
    }

    private class SecondaryAbility
    {
        private float damage;
        private float cooldown;
        private float timeTillReady = 0;
        private float projectileSpeed;
        private float maxDistance;
        private GameObject projectile;
        private Transform shootFrom;

        public SecondaryAbility(ref SecondaryAbConfig conf, GameObject projectile, Transform shootFrom)
        {
            this.projectile = projectile;
            this.shootFrom = shootFrom;

            damage = conf.damage;
            cooldown = 1 / conf.attacksPerSecond;
            projectileSpeed = conf.projectileSpeed;
            maxDistance = conf.maxRange;
        }

        public void Update(ref RaycastHit mouseToWorld, bool hit)
        {
            if (timeTillReady > 0)
                timeTillReady -= Time.deltaTime;

            if (timeTillReady <= 0 && Input.GetKey(KeyCode.Mouse0))
            {
                timeTillReady += cooldown;
                //if (hit)
                //    ShootPrecise(ref mouseToWorld);
                //else
                    Shoot();
            }
                
        }
        void ShootPrecise(ref RaycastHit hit)
        {
            var proj = CreateProjectileInstance().transform;
            proj.rotation = Quaternion.LookRotation(hit.point - shootFrom.position);
        }

        void Shoot()
        {
            var proj = CreateProjectileInstance().transform;
            proj.rotation = shootFrom.rotation;
        }

        private GameObject CreateProjectileInstance()
        {
            var projectileInstance = Instantiate(projectile);
            projectileInstance.transform.position = shootFrom.position;
            var bull = projectileInstance.GetComponent<Bullet>();
            bull.damage = damage;
            bull.target = EObjectType.Enemy;
            bull.speed = projectileSpeed;
            bull.maxDistance = maxDistance;
            bull.origin = shootFrom.position;
            return projectileInstance;
        }
    }

    private class Health
    {
        private float maxHealth = 0;
        private float health = 0;
        public Health(float maxHealth) => health = this.maxHealth = maxHealth;        
        public bool IsDepleted => health <= 0;
        public void ResetHealth() => health = maxHealth;
        public void TakeDmg(float amount) => health = Mathf.Max(0, health - amount);
        public void Heal(float amount) => health = Mathf.Min(maxHealth, health + amount);
    }

    public void ReceiveDamage(float amount)
    {
        Debug.Log("Taking damage: "+ amount);
        health.TakeDmg(amount);
        if (health.IsDepleted) Destroy(gameObject);
    }

    public EObjectType GetObjectType() => EObjectType.Player;
}
