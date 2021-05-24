using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DashAbility : MainAbility
{
    public DashAbilityConfig conf { get => internalConf; set { internalConf = value; mainAbilityConfig = value.baseConfig; } }

    private DashAbilityConfig internalConf;

    public override void CastAbility()
    {
        base.CastAbility();
        playerController.StartCoroutine(PerformDash());
    }

    IEnumerator PerformDash()
    {
        playerController.moveStop = true;
        Debug.Log("Dashing");
        Transform playerTrans = playerController.transform;
        Vector3 start = playerTrans.position;
        Vector3 end = playerController.mouseWorldPosition;
        end.y += 1.2f;
        Vector3 direction = (end - start);
        float distance = Mathf.Min(direction.magnitude, conf.maxRange);
        direction.Normalize();

        float actDist;
        float halfDist = distance /2;
        while ((actDist = Vector3.Distance(playerTrans.position, start)) < distance)
        {
            if(actDist < distance / 2)
            {
                float scaler = (1 - actDist / halfDist);
                playerTrans.localScale = Vector3.one * (scaler * scaler * scaler);
            }
            else
            {
                float scaler = ((actDist - halfDist) / halfDist);
                playerTrans.localScale = Vector3.one * (scaler * scaler * scaler);
            }
            playerTrans.position += direction * conf.speed * Time.deltaTime;
            yield return null;
        }
        playerTrans.localScale = Vector3.one;
        playerTrans.position = start +  direction * distance;       
        playerController.moveStop = false;
    }
}
