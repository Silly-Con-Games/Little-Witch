using System;
using UnityEngine;
using Config;
using Assets.Scripts.GameEvents;

[Serializable]
public class ChargeAbility : Ability
{

    public Transform abilityAnimationTransform;
    public Transform origin;
    public PlayerController parent;
    public ChargeAbilityProjectile projectilePrefab;

    public ChargeAbilityConfig conf;

    private float startedChargeTime = 0.0f;
    private float lastFireTime = float.NegativeInfinity;

    public bool IsCharging { get; internal set; }

    public void UpdateAnimation()
    {
        if (!IsCharging)
            return;

        float chargePercent = GetChargedTime() / conf.chargeTimeMax;

        float localScale = Mathf.Lerp(conf.spawnRadiusMin, conf.spawnRadiusMax, chargePercent);
        abilityAnimationTransform.localScale = new Vector3(localScale, localScale, localScale);
    }

    public bool IsReady()
    {
        if (IsCharging || !parent.energy.HasEnough(conf.energyCost))
            return false;
        else
            return Time.time - lastFireTime > conf.cooldown;
    }
    public void StartCharge()
    {
        if (IsReady())
        {
            onPerformed.Invoke(AbilityType.Charge);
            IsCharging = true;
            startedChargeTime = Time.time;
            parent.ScaleSpeedModifier(conf.witchSlowMSMultiplier);
            parent.energy.UseEnergy(conf.energyCost);
            GameEventQueue.QueueEvent(new ChargeAbilityEvent(cast: true, energyCost: conf.energyCost));

            abilityAnimationTransform.localScale = new Vector3(conf.spawnRadiusMin, conf.spawnRadiusMin, conf.spawnRadiusMin);
        }            
    }

    public void FireCharged()
    {
        if (!IsCharging)
            Debug.LogError("How can you fire charged when you are not charging");

        lastFireTime = Time.time;
        IsCharging = false;
        abilityAnimationTransform.localScale = Vector3.zero;
        parent.ScaleSpeedModifier(1 / conf.witchSlowMSMultiplier);

        float chargePercent = GetChargedTime()/ conf.chargeTimeMax;

        var instance = GameObject.Instantiate(projectilePrefab);
        instance.transform.position = origin.position;
        instance.transform.rotation = origin.rotation;
        float localScale = Mathf.Lerp(conf.spawnRadiusMin, conf.spawnRadiusMax, chargePercent);
        instance.transform.localScale = new Vector3(localScale, localScale, localScale);

        instance.origin = origin.position;
        instance.damage = Mathf.Lerp(conf.damageMin, conf.damageMax, chargePercent);
        instance.explosionSpeed = Mathf.Lerp(conf.explosionSpeedMin, conf.explosionSpeedMax, chargePercent);
        instance.distance = Mathf.Lerp(conf.distanceMin, conf.distanceMax, chargePercent);
        instance.speed = Mathf.Lerp(conf.speedMin, conf.speedMax, chargePercent);
        instance.explosionRadius = Mathf.Lerp(conf.explosionRadiusMin, conf.explosionRadiusMax, chargePercent);
    }

    private float GetChargedTime()
    {
        return Mathf.Min(Time.time - startedChargeTime, conf.chargeTimeMax);
    }
}