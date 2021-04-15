using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAbilityProjectile : MonoBehaviour
{
    public EObjectType target = EObjectType.Enemy;
    public float damage = 0f;
    public float speed = 0f;
    public float distance = 0;
    public Vector3 origin;
    public float explosionRadius;
    public float explosionSpeed;

    private bool isExploding;
    private void Update()
    {
        if (isExploding)
        {
            transform.localScale += new Vector3(Time.deltaTime * explosionSpeed, Time.deltaTime * explosionSpeed, Time.deltaTime * explosionSpeed);
            if (transform.localScale.x > explosionRadius)
                Destroy(gameObject);
            return;
        }
        float delta = Time.deltaTime;
        transform.position += transform.forward * delta * speed;

        if (distance > 0 && Vector3.Distance(origin, transform.position) >= distance)
            isExploding = true;
    }

    private void OnTriggerEnter(Collider collision)
    {
        var other = collision.gameObject;
        var damagable = other.GetComponent<IDamagable>();
        if (damagable == null)
        {
            isExploding = true;
        }
        else if (damagable.GetObjectType() == target)
        {
            Debug.Log("Charge hit target!");
            damagable.ReceiveDamage(damage);
            isExploding = true;
        }
    }
}
