using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Animator swing;

    public float Damage;

    HashSet<GameObject> hitObjects = new HashSet<GameObject>();

    EObjectType target = EObjectType.Enemy;

    public void Attack()
    {
        swing.SetTrigger("Swing");
        hitObjects.Clear();
    }

    private void OnTriggerEnter(Collider collision)
    {
        var other = collision.gameObject;
        if (hitObjects.Contains(other))
            return;
        hitObjects.Add(other);
        var damagable = other.GetComponent<IDamagable>();

        if (damagable?.GetObjectType() == target)
        {
            Debug.Log("SwingAttack hit target!");
            damagable.ReceiveDamage(Damage);
        }
    }

}
