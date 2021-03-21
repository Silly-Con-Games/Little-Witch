using UnityEngine;

public class Bullet : MonoBehaviour
{
    public EType target = EType.Unknown;
    public float damage = 0f;
    public float speed = 0f;
    public float lifetime = 5;
    private void Update()
    {
        float delta = Time.deltaTime;
        transform.position += transform.forward * delta * speed;

        lifetime -= delta;
        if(lifetime < 0)
            Destroy(gameObject);
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        var other = collision.gameObject;
        var damagable = other.GetComponent<IDamagableObject>();
        if(damagable == null)
        {
            Destroy(gameObject);
        }            
        else if (damagable.GetType() == target)
        {
            Debug.Log("Bullet hit target!");
            damagable.ReceiveDamage(damage);
            Destroy(gameObject);
        }        
    }
}
