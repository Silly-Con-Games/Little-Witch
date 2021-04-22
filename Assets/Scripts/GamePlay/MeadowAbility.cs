using UnityEngine;
using System;

[Serializable]
public class MeadowAbility : MainAbility
{
    public override void CastAbility()  
    {
        base.CastAbility();
        Debug.Log("Casted meadow ability!");
    }
}
