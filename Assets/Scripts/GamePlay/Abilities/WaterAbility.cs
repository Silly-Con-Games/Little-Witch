using UnityEngine;
using System;
using Config;
using UnityEngine.Assertions;

[Serializable]
public class WaterAbility : MainAbility
{
    public WAWave wavePrefab;

    public WaterAbilityConfig conf { get => internalConf; set { internalConf = value; mainAbilityConfig = value.baseConfig; } }
    WaterAbilityConfig internalConf;

    public override void CastAbility()
    {
        base.CastAbility();
        Debug.Log("Casted water ability!");
        var inst = GameObject.Instantiate(wavePrefab).transform;
        inst.position = playerController.transform.position;
        inst.rotation = playerController.transform.rotation;
    }

    public void PassiveHealEffect()
    {
        playerController.health.Heal(conf.healPerSec * Time.deltaTime);
    }

    public void SteppedOnWater()
    {
        Assert.IsFalse(playerController.passiveEffects.Contains(PassiveHealEffect));
        playerController.passiveEffects.Add(PassiveHealEffect);
    }

    public void SteppedFromWater()
    {
        Assert.IsTrue(playerController.passiveEffects.Contains(PassiveHealEffect));
        playerController.passiveEffects.Remove(PassiveHealEffect);
    }
}
