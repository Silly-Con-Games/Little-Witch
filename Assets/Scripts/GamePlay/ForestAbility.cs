using UnityEngine;
using System;

[Serializable]
public class ForestAbility : MainAbility
{
    public override void CastAbility()
    {
        base.CastAbility();
        Debug.Log("Casted forest ability!");
    }
}
