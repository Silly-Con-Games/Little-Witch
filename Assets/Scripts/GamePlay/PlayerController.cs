using System;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamagable
{
    public CharacterController characterController;
    public GameObject bulletPrefab;
    public float speed = 3;
    public float health = 20;

    public float jumpHeight = 1.0f;

    private const float gravity = -9.81f;
    private float upVelocity = 0;

    private Camera mainCamera;
    private Transform cameraTrans;

    private void Start()
    {
        mainCamera = Camera.main;
        cameraTrans = mainCamera.transform;
    }
    // Update is called once per frame
    void Update()
    {
        MoveUpdate();

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        bool didHit = false;
        if (didHit = Physics.Raycast(ray, out hit, 1000))
        {
            var targetPosition = hit.point;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
        }


        if (Input.GetKeyDown(KeyCode.Mouse0))
            if (didHit)
                ShootPrecise(hit);
            else
                Shoot();
    }



    void MoveUpdate()
    {
        Vector3 forwardV = cameraTrans.forward;
        forwardV.y = 0;
        forwardV.Normalize();
        Vector3 rightV = cameraTrans.right;
        Vector3 velocity = Vector3.zero;
        float delta = Time.deltaTime;

        if (Input.GetKey(KeyCode.D))
            velocity += rightV;
        if (Input.GetKey(KeyCode.A))
            velocity -= rightV;

        if (Input.GetKey(KeyCode.W))
            velocity += forwardV;
        if (Input.GetKey(KeyCode.S))
            velocity -= forwardV;

        velocity.Normalize();
        velocity *= speed;

        if (characterController.isGrounded)
        {
            upVelocity = 0;
            if (Input.GetKey(KeyCode.Space))
                upVelocity += Mathf.Sqrt(jumpHeight * -3f * gravity);           
        }
        else
            upVelocity += gravity * delta;

        velocity.y = upVelocity;
        Debug.Log("Velocity is" + velocity);

        if (velocity.magnitude > 0)
            characterController.Move(velocity * delta);
    }

    private void ShootPrecise(RaycastHit hit)
    {
        var bulletInstanceTrans = Instantiate(bulletPrefab).transform;
        bulletInstanceTrans.position = transform.position;
        bulletInstanceTrans.rotation = Quaternion.LookRotation( hit.point - transform.position );
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

    public EObjectType GetObjectType() => EObjectType.Player;
}
