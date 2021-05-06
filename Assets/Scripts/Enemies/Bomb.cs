using System.Collections;
using System.Collections.Generic;
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
        bombCollider = gameObject.GetComponent<Collider>();
        explosionDelay = 5f;
        damageRange = 5f;
        baseDamage = 40;
        disappearingDuration = 1f;
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
        Destroy(gameObject);
    }
    
}
