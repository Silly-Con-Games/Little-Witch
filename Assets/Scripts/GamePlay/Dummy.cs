using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour, IDamagableObject
{
    public EType Type = EType.Enemy;
    public float Health = 10;
    public void ReceiveDamage(float amount)
    {
        Debug.Log($"I'm hit ({amount}) - {gameObject.name}");

        if ((Health -= amount) <= 0)
        {
            Debug.Log($"I died :( - {gameObject.name}");
            Destroy(gameObject);
        };
    }

    EType IDamagableObject.GetType() => Type;

}
