using System.Collections;
using System.Collections.Generic;
using Config;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    
    [SerializeField]
    private ParticleSystem particle;
    
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
        GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);
        bombCollider = gameObject.GetComponent<Collider>();
        ApplyConfig();
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
        StartCoroutine(BombCoroutine(explosionDelay));
    }

    public IEnumerator BombCoroutine(float duration)
    {
        while (duration >= 0f)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        particle.Play();

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
