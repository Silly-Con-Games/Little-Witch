using UnityEngine;
using System;
using Config;

[Serializable]
public abstract class MainAbility : Ability
{
    protected float lastUsedTime = float.NegativeInfinity;

    protected MainAbilityConfig mainAbilityConfig;

    protected PlayerController playerController;

    public void Init(PlayerController parent)
    {
        playerController = parent;
    }

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
