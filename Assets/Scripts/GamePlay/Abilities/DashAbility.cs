using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DashAbility : MainAbility
{
    public DashAbilityConfig conf { get => internalConf; set { internalConf = value; mainAbilityConfig = value.baseConfig; } }
    public float playerHeight = 1.2f;
    public TrailRenderer dashEffect;
    private DashAbilityConfig internalConf;

    public override void CastAbility()
    {
        base.CastAbility();
        playerController.StartCoroutine(PerformDash());
    }

    IEnumerator PerformDash()
    {
        dashEffect.emitting = true;
        playerController.moveStop = true;
        Transform playerTrans = playerController.transform;
        Vector3 start = playerTrans.position;
        Vector3 end = playerController.mouseWorldPosition;
        end.y += playerHeight;
        if (end.y < 0.35f)
            end.y = playerHeight;
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
        dashEffect.emitting = false;
    }
}
