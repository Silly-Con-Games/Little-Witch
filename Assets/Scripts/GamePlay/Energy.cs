using System.Collections;
using System.Collections.Generic;
using Config;
using UnityEngine;

public class Energy : MonoBehaviour
{

    private float lifeTimeInSec;
    private PlayerController playerController;
    private float speed;
    private int energyAmount;
        
    void Start()
    {
        GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);
        ApplyConfig();
        StartCoroutine(WaitCoroutine());
    }

    protected virtual void ApplyConfig()
    {
        var energyConfig = GlobalConfigManager.GetGlobalConfig().energyConfig;
        lifeTimeInSec = energyConfig.lifeTimeInSec;
        speed = energyConfig.speed;
        energyAmount = energyConfig.energyAmount;

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
        GlobalConfigManager.onConfigChanged.RemoveListener(ApplyConfig);
        FMODUnity.RuntimeManager.PlayOneShot("event:/energy/collect", transform.position);
        Destroy(gameObject);
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(lifeTimeInSec);
        Destroy(gameObject);
    }


}
