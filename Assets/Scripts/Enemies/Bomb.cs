using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    
    [SerializeField]
    private ParticleSystem particle;
    
    [SerializeField]
    private PlayerController playerController;

    private bool bombActivated;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!particle)
        {
            particle.GetComponentInChildren<ParticleSystem>();
        }
        if (!playerController)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        bombActivated = false;
    }

    void OnTriggerEnter(Collider collision)
    {
<<<<<<< Updated upstream
        if (!bombActivated)
        {
            bombActivated = true;
            StartCoroutine(BombCoroutine(5f));
        }
=======
        playerController = collision.gameObject.GetComponent<PlayerController>();
        bombCollider.enabled = false;        
        StartCoroutine(BombCoroutine(.3f));
>>>>>>> Stashed changes
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

            if (distance <= 5f)
            {
                playerController.ReceiveDamage(40 * (1 - distance / 5f));
            }
        }
        
        
        duration = 1f;
        while (duration >= 0f)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
    
}
