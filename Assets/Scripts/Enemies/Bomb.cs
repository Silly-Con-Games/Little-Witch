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
    
    private PlayerController playerController;

    private Collider bombCollider;

    private float explosionDelay;
    private float damageRange;
    private float baseDamage;
    private float disappearingDuration;
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (!particle)
        {
            particle.GetComponentInChildren<ParticleSystem>();
        }

        if (!animator)
        {
            gameObject.GetComponentInChildren<Animator>();
        }
        GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);
        bombCollider = gameObject.GetComponent<Collider>();
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
    }


    void OnTriggerEnter(Collider collision)
    {
        playerController = collision.gameObject.GetComponent<PlayerController>();
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
        if (playerController)
        {
            float distance = Vector3.Distance(this.transform.position, playerController.transform.position);

            if (distance <= damageRange)
            {
                playerController.ReceiveDamage(baseDamage * (1f - distance / damageRange));
            }
        }
        
        duration = disappearingDuration;
        while (duration >= 0f)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        GlobalConfigManager.onConfigChanged.RemoveListener(ApplyConfig);
        Destroy(gameObject);
    }
    
}
