using UnityEngine;
using System;
using Config;
using UnityEngine.Assertions;
using UnityEngine.VFX;

[Serializable]
public class WaterAbility : MainAbility
{
    public WAWave wavePrefab;
    public VisualEffect healingEffect;

    public WaterAbilityConfig conf { get => internalConf; set { internalConf = value; mainAbilityConfig = value.baseConfig; } }
    WaterAbilityConfig internalConf;

    public override void CastAbility()
    {
        base.CastAbility();
        Debug.Log("Casted water ability!");
        var inst = GameObject.Instantiate(wavePrefab);
        var instTrans = inst.transform;
        instTrans.position = playerController.transform.position;
        instTrans.rotation = playerController.transform.rotation;
        inst.Init(ref internalConf);

    }

    public void PassiveHealEffect()
    {
        playerController.health.Heal(conf.healPerSec * Time.deltaTime);

    }

    public void SteppedOnWater()
    {
        Assert.IsFalse(playerController.passiveEffects.Contains(PassiveHealEffect));
        playerController.passiveEffects.Add(PassiveHealEffect);
        healingEffect.SendEvent("OnPlay");
    }

    public void SteppedFromWater()
    {
        Assert.IsTrue(playerController.passiveEffects.Contains(PassiveHealEffect));
        playerController.passiveEffects.Remove(PassiveHealEffect);
        healingEffect.SendEvent("OnStop");

    }
}
