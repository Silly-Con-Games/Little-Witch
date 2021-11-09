using System;
using UnityEngine;
using Config;
using Assets.Scripts.GameEvents;

[Serializable]
public class MeleeAbility
{
    public SwordAttack swing;
    public MeleeAbilityConfig conf 
        { 
          get => internalConf; 
          set 
            { 
                internalConf = value; 
                swing.Init(player.transform, 
                        internalConf.damage, 
                        internalConf.pushbackForce, 
                        internalConf.pushbackDuration
                );
            } 
        }
    MeleeAbilityConfig internalConf;
    public PlayerController player;

    private float timeSinceLastAttack = float.NegativeInfinity;

    public bool IsReady => Time.time - timeSinceLastAttack > conf.cooldown;

    public void Attack()
    {
        if (IsReady)
        {
            timeSinceLastAttack = Time.time;
            swing.Attack();
            GameEventQueue.QueueEvent(new MeleeAbilityEvent(cast : true));
        }
    }
}

