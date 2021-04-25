using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour
{

    private float lifeTimeInSec;
    private PlayerController playerController;
    private float speed;
        
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
            if (playerController.energy < playerController.energyMax)
            {
                StartCoroutine(FollowPlayerCoroutine());
            }
        }
    }

    IEnumerator FollowPlayerCoroutine()
    {
        while (playerController && Vector3.Distance(transform.position, playerController.transform.position) >= 0.5f)
        {
            if (playerController.energy >= playerController.energyMax)
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
        playerController.AddEnergy(1);
        Debug.Log("Energy count: " + playerController.energy);
        Destroy(gameObject);
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(lifeTimeInSec);
        Destroy(gameObject);
    }


}
