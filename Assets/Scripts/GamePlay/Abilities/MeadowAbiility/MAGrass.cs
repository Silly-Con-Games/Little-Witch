using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAGrass : MonoBehaviour
{

    Vector3 velocity;
    EObjectType target = EObjectType.Enemy;
    float damage;
    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamagable dmg = other.gameObject.GetComponent<IDamagable>();
        if (dmg != null && dmg.GetObjectType() == target)
            dmg.ReceiveDamage(damage);

    }
}
