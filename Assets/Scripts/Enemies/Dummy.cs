using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour, IDamagable, IRootable
{
    public EObjectType Type = EObjectType.Enemy;
    public float Health = 10;

    public void ReceiveDamage(float amount)
    {
        Debug.Log($"I'm hit ({amount}) - {gameObject.name}");

        if ((Health -= amount) <= 0)
        {
            Debug.Log($"I died :( - {gameObject.name}");
            Destroy(gameObject);
        }
    }

    public void ReceiveRoot(float duration)
    {
        Debug.Log($"I cannot move for {duration} seconds - {gameObject.name}");
    }

    EObjectType IObjectType.GetObjectType() => Type;
}
