using UnityEngine;

public class PlayerController : MonoBehaviour, IDamagableObject
{
    public GameObject bulletPrefab;
    public float speed = 3;
    public float health = 20;
	
    void Update()
    {
        Vector3 velocity = Vector3.zero;
        float delta = Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            velocity += new Vector3(1, 0, 0);
        if (Input.GetKey(KeyCode.A))
            velocity += new Vector3(-1, 0, 0);

        if (Input.GetKey(KeyCode.W))
            velocity += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.S))
            velocity += new Vector3(0, 0, -1);

        if (velocity.magnitude > 0)
            transform.position += (velocity.normalized) * delta * speed;


        if (Input.GetKeyDown(KeyCode.Mouse0))
            Shoot();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000))
        {
            var targetPosition = hit.point;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
        }
    }

    void Shoot()
    {
        var bulletInstanceTrans = Instantiate(bulletPrefab).transform;
        bulletInstanceTrans.position = transform.position;
        bulletInstanceTrans.rotation = transform.rotation;
    }

    public void ReceiveDamage(float amount) 
    {
        if ((health -= amount) <= 0) Destroy(gameObject);
    }

    EType IDamagableObject.GetType() => EType.Player;
}
