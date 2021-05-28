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
    private float rad = 0;
    private bool isExploding;
    private void Update()
    {
        if (isExploding)
        {
            rad += Time.deltaTime * explosionSpeed;
            float alp = 1 - rad / explosionRadius;
            alp = Mathf.Clamp(alp,0,1);
            transform.localScale = Vector3.one * ( 1 - (alp * alp ) ) * explosionRadius;
            
            if (alp == 0)
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
