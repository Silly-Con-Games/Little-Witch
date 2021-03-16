using UnityEngine;

public class Bullet : MonoBehaviour
{
    public EType target = EType.Unknown;
    public float damage = 0f;
    

    private void OnCollisionEnter(Collision collision)
    {
        var damagable = collision.gameObject.GetComponent<IDamagableObject>();
        if (damagable?.GetType() == target)
        {
            damagable.DealDamage(damage);
            Destroy(gameObject);
        }
    }
}
