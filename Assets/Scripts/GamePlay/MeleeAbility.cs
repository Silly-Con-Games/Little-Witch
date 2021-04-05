using System;
using UnityEngine;
using Config;

[Serializable]
public class MeleeAbility
{
    public SwordAttack swing;
    public MeleeAbilityConfig conf;

    private float timeSinceLastAttack = -20;

    public bool IsReady => Time.time - timeSinceLastAttack > conf.cooldown;

    public void Attack()
    {
        if (IsReady)
        {
            swing.Attack();
            swing.Damage = conf.damage;
        }
    }
}

