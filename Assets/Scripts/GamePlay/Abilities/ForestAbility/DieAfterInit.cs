using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieAfterInit : MonoBehaviour
{
    public void Init(float duration)
    {
        StartCoroutine(DieAfterDuration(duration));
    }

    IEnumerator DieAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
