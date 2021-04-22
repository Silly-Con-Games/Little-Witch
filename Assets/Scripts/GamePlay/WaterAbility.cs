using UnityEngine;
using System;

[Serializable]
public class WaterAbility : MainAbility
{
    public override void CastAbility()
    {
        base.CastAbility();
        Debug.Log("Casted water ability!");
    }
}
