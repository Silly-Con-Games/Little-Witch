using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour
{

    private float lifeTimeInSec;
    private PlayerController playerController;
    private float speed;
    private float energyAmount = 1;
        
    void Start()
    {
        speed = 1f;
        lifeTimeInSec = 15;
        StartCoroutine(WaitCoroutine());
    }

    void OnTriggerEnter(Collider collision)
    {
        playerController = collision.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            if (playerController.energy.CanFitMore)
            {
                StartCoroutine(FollowPlayerCoroutine());
            }
        }
    }

    IEnumerator FollowPlayerCoroutine()
    {
        while (playerController && Vector3.Distance(transform.position, playerController.transform.position) >= 0.5f)
        {
            if (!playerController.energy.CanFitMore)
            {
                yield break;
            }
            transform.position = Vector3.MoveTowards(
                transform.position, 
                playerController.transform.position, 
                speed * Time.deltaTime
                );
                yield return null;
        }
        playerController.energy.AddEnergy(energyAmount);
        Destroy(gameObject);
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(lifeTimeInSec);
        Destroy(gameObject);
    }


}
