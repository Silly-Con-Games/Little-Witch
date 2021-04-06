using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomUtils
{

    public static Vector3 GetRandomDirection()
    {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f),0f,UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

}
