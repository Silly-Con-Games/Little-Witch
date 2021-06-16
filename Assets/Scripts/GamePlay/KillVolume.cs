using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IDamagable damagable = other.gameObject.GetComponent<IDamagable>();
        if (damagable != null)
            damagable.ReceiveDamage(float.MaxValue);
        else
            Destroy(other.gameObject);
    }
}
