using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            gameObject.transform.position += new Vector3(10 * delta, 0, 0);
        if (Input.GetKey(KeyCode.A))
            gameObject.transform.position += new Vector3(-10 * delta, 0, 0);

        if (Input.GetKey(KeyCode.W))
            gameObject.transform.position += new Vector3(0, 0, 10 * delta);
        if (Input.GetKey(KeyCode.S))
            gameObject.transform.position += new Vector3(0, 0, -10 * delta);
    }
}
