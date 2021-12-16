using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class TileBridge : MonoBehaviour
{
    public float ascentHeight = 0;
    public float descentHeight = -5;
    public float moveSpeed = 4;

    public bool IsLowered = false;

    private Coroutine cor;
    public void Descent()
    {
        IsLowered = true;
        if(cor != null)
            StopCoroutine(cor);

        cor = StartCoroutine(MoveCor(descentHeight));
    }

    private IEnumerator MoveCor(float targetHeight)
    {
        float height = transform.position.y;
        Func<float, bool> condition;
        float mutliplier; 
        if (height < targetHeight)
        {
            condition = x => x < targetHeight;
            mutliplier = 1;
        }
        else if (height > targetHeight)
        {
            condition = x => x > targetHeight;
            mutliplier = -1;
        }
        else
            yield break;

        while (condition(height))
        {
            height += Time.deltaTime * moveSpeed * mutliplier;
            if (!condition(height))
                height = targetHeight;

            Vector3 pos = transform.position;
            pos.y = height;
            transform.position = pos;
            yield return null;
        }
        cor = null;
    }

    public void Ascent()
    {
        IsLowered = false;
        if (cor != null)
            StopCoroutine(cor);

        cor = StartCoroutine(MoveCor(ascentHeight));
    }

    private void OnValidate()
    {
        if (IsLowered)
        {
            Vector3 pos = transform.position;
            pos.y = descentHeight;
            transform.position = pos;
        }
        else
        {
            Vector3 pos = transform.position;
            pos.y = ascentHeight;
            transform.position = pos;
        }
    }
}
