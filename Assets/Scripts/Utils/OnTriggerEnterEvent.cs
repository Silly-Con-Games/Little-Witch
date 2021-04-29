using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterEvent : MonoBehaviour
{
    public UnityEvent<Collider> ontriggerenter;

    private void OnTriggerEnter(Collider other)
    {
        ontriggerenter.Invoke(other);
    }

    private void OnDestroy()
    {
        ontriggerenter.RemoveAllListeners();
    }
}
