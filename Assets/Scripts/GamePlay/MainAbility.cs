using UnityEngine;
using System;
using Config;

[Serializable]
public abstract class MainAbility
{
    protected float lastUsedTime = float.NegativeInfinity;
    [SerializeField]
    protected MainAbilityConfig mainAbilityConfig;

    public virtual void CastAbility()
    {
        lastUsedTime = Time.time;
    }

    public virtual bool IsReady => Time.time - lastUsedTime > mainAbilityConfig.cooldown;

    public virtual float ChargedInPercent()
    {
        if (mainAbilityConfig.cooldown == 0)
            return 1.0f;
        return Mathf.Clamp((Time.time - lastUsedTime) / mainAbilityConfig.cooldown, 0.0f,1.0f);
    }
}
