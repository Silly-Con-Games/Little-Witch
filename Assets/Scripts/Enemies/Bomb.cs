using System.Collections;
using System.Collections.Generic;
using Config;
using UnityEngine;

public class Bomb : MonoBehaviour
{ 
    [SerializeField]
    private ParticleSystem particle;

    [SerializeField]
    private Animator animator;

    private Collider bombCollider;

    private float explosionDelay;
    private float damageRange;
    private float baseDamage;
    private float disappearingDuration;

    private static int mask = -1;

    // Start is called before the first frame update
    void Start()
    {
        if(mask == -1)
        {
            mask = LayerMask.GetMask("Enemy", "Character");
        }

        if (!particle)
        {
            particle.GetComponentInChildren<ParticleSystem>();
        }

        if (!animator)
        {
            gameObject.GetComponentInChildren<Animator>();
        }
		
		bombCollider = GetComponent<SphereCollider>();
		GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);
        ApplyConfig();
        animator.enabled = false;
    }

    protected virtual void ApplyConfig()
    {
        var enemyConfig = GlobalConfigManager.GetGlobalConfig().globalEnemyConfig.mineConfig;
        explosionDelay = enemyConfig.explosionDelay;
        damageRange = enemyConfig.damageRange;
        baseDamage = enemyConfig.baseDamage;
        disappearingDuration = enemyConfig.disappearingDuration;

		float diameter = damageRange * 2;
		transform.localScale = new Vector3(diameter, diameter, diameter);
		ParticleSystem.VelocityOverLifetimeModule module = particle.velocityOverLifetime;
		module.speedModifier = diameter;
    }


    void OnTriggerEnter(Collider collision)
    {
        bombCollider.enabled = false;
        FMODUnity.RuntimeManager.PlayOneShot("event:/enemies/mine/trigger", transform.position);
        StartCoroutine(BombCoroutine(explosionDelay));
    }

    public IEnumerator BombCoroutine(float duration)
    {
        animator.enabled = true;
        while (duration >= 0f)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        FMODUnity.RuntimeManager.PlayOneShot("event:/enemies/mine/explosion", transform.position);
        particle.Play();
        animator.enabled = false;

        var colliders = Physics.OverlapSphere(transform.position, damageRange, mask);
        foreach(var collider in colliders)
        {
            var damageble = collider.GetComponent<IDamagable>();
            if(damageble != null)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                float dmg = baseDamage * (1f - distance / damageRange);
                if(dmg > 0)
                    damageble.ReceiveDamage(dmg);
            }
        }
        
        duration = disappearingDuration;
        while (duration >= 0f)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        GlobalConfigManager.onConfigChanged.RemoveListener(ApplyConfig);
        Destroy(gameObject.transform.parent.gameObject);
    }
    
}
