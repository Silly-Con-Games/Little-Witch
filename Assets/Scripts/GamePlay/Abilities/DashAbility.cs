using Assets.Scripts.GameEvents;
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
    public float dashLengthModifier = 1;

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
        MapController mapController = playerController.mapController;
        Vector3 start = playerTrans.position;
        Vector3 direction = new Vector3(playerController.inputVelocity.x, 0, playerController.inputVelocity.y);
        float distance = conf.maxRange * dashLengthModifier;

        if (direction.sqrMagnitude == 0)
        {
            Vector3 end = playerController.mouseWorldPosition;
            end.y += playerHeight;
            if (end.y < 0.35f)
                end.y = playerHeight;
            direction = (end - start);
            distance = conf.maxRange * dashLengthModifier;
            direction.Normalize();
        }
        GameEventQueue.QueueEvent(new DashAbilityEvent(distance));

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

            Vector3 pos = playerTrans.position + direction * conf.speed * Time.deltaTime;
            pos.y = playerHeight;
            float tileHeight = mapController.TileHeightInPosition(playerTrans.position);
            if (!float.IsNaN(tileHeight))
                pos.y += tileHeight;

            playerTrans.position = pos;
            yield return null;
        }
        playerTrans.localScale = Vector3.one;

        Vector3 posAf = start + direction * distance;
        posAf.y = playerHeight;

        float tileHeightAf = mapController.TileHeightInPosition(posAf);
        if (!float.IsNaN(tileHeightAf))
        {
            posAf.y += tileHeightAf;
        }
        playerTrans.position = posAf;

        playerController.moveStop = false;
        playerController.characterController.Move(Vector3.zero); // Hack, character controller needs to move to update "isGrounded" (if not updated character can get stuck in the air)
        dashEffect.emitting = false;
    }
}
