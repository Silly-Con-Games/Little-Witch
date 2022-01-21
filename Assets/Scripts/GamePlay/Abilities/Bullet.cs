using UnityEngine;

public class Bullet : MonoBehaviour , IObjectType
{
    public EObjectType target = EObjectType.Unknown;
    public float damage = 0f;
    public float speed = 0f;
    public float maxDistance = 0;
    public Vector3 origin;
    private void Update()
    {
        float delta = Time.deltaTime;
        transform.position += transform.forward * delta * speed;

        if(maxDistance > 0 && Vector3.Distance(origin, transform.position) >= maxDistance)
            Destroy(gameObject);
    }
    
    private void OnTriggerEnter(Collider coll)
    {
        var type = coll.GetComponent<IObjectType>();
        if (type != null && type.GetObjectType() == EObjectType.Projectile)
            return;

        var damagable = coll.GetComponent<IDamagable>();
        if(damagable == null)
        {
            Destroy(gameObject);
        }            
        else if (damagable.GetObjectType() == target)
        {
            //Debug.Log("Bullet hit target!");
            damagable.ReceiveDamage(damage);
            Destroy(gameObject);
        }        
    }

    public EObjectType GetObjectType()
    {
        return EObjectType.Projectile;
    }
}
