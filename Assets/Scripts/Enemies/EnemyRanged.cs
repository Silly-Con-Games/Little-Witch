using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : EnemyAI
{

    public GameObject bulletPrefab;

    public override void Attack()
    {
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.GetComponentInChildren<Bullet>().target = EType.Player;
        var bulletInstanceTrans = bullet.transform;
        bulletInstanceTrans.position = transform.position;
        bulletInstanceTrans.rotation = transform.rotation;
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
