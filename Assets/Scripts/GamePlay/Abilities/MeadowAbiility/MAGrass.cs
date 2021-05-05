using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MAGrass : MonoBehaviour
{
    CatmulRollSpline spline;

    float t = 0;
    float speed = 0.5f;
    UnityAction<Collider> onHit;

    public void Init(CatmulRollSpline spline, float speed, UnityAction<Collider> onHit)
    {
        this.spline = spline;
        this.speed = speed;
        this.onHit = onHit;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime * speed;
        if(t <= 1) 
        {
            transform.localPosition = spline.PointAt(t);
            transform.localRotation = Quaternion.LookRotation(spline.FirstDerivativeAt(t));
        }
        else
        {
            
            if(transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        onHit?.Invoke(other);
    }
}
