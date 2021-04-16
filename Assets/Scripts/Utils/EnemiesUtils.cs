using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesUtils
{
    public static Vector3 GetRoamPosition(Vector3 startingPosition, float moveRangeMin, float moveRangeMax)
    {
        return startingPosition + RandomUtils.GetRandomDirection() * Random.Range(moveRangeMin, moveRangeMax);
    }
    
    

}
