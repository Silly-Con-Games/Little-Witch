using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : EnemyAI
{
    public override void Attack()
    {
        if (playerController)
        {
            Debug.Log("Attacking");
            playerController.ReceiveDamage(3);
        }
    }

    public override bool Stun()
    {
        throw new System.NotImplementedException();
    }

    public override bool Slow()
    {
        throw new System.NotImplementedException();
    }

    public override bool Root()
    {
        throw new System.NotImplementedException();
    }
}
