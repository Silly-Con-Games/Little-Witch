using UnityEngine;
using System;
using Config;

[Serializable]
public class MeadowAbility : MainAbility
{
    public MeadowAbilityConfig conf { get => internalConf; set { internalConf = value; mainAbilityConfig = value.baseConfig; } }

    private MeadowAbilityConfig internalConf;

    public override void CastAbility()  
    {
        base.CastAbility();
        Debug.Log("Casted meadow ability!");
    }

    public void SteppedOnMeadow()
    {
        playerController.ScaleSpeedModifier(conf.MSMultiplier);
    }

    public void SteppedFromMeadow()
    {
        playerController.ScaleSpeedModifier(1/conf.MSMultiplier);
    }
}
