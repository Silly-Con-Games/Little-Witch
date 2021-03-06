using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Animator swing;
    public Collider col;
    public TrailRenderer trail;
    float damage;
    float pushForce;
    float pushDuration;
    Transform hitOrigin;

    HashSet<GameObject> hitObjects = new HashSet<GameObject>();

    EObjectType target = EObjectType.Enemy;

    public void Init(Transform hitOrigin, float damage, float pushForce, float pushDuration)
    {
        this.damage = damage;
        this.pushForce = pushForce;
        this.pushDuration = pushDuration;
        this.hitOrigin = hitOrigin;
    }
    public void Attack()
    {
        swing.SetTrigger("Swing");
        hitObjects.Clear();
        StartCoroutine(disableColliderCouroutine());
        StartCoroutine(enableColliderCouroutine());
        StartCoroutine(disableTrailCouroutine());
    }

    IEnumerator disableColliderCouroutine()
    {
        yield return new WaitForSeconds(0.4f);

        col.enabled = false;
    }
    IEnumerator enableColliderCouroutine()
    {
        yield return new WaitForSeconds(0.15f);
        trail.emitting = true;

        col.enabled = true;
    }

    IEnumerator disableTrailCouroutine()
    {
        yield return new WaitForSeconds(0.4f);
        trail.emitting = false;

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
            damagable.ReceiveDamage(damage);
        }

        var pushable = other.GetComponent<IPushable>();

        if (pushable?.GetObjectType() == target)
        {
            Vector3 force = (other.transform.position - hitOrigin.position).normalized * pushForce;
            pushable.ReceivePush(force, pushDuration);
        }
    }

}
