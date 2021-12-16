using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ability
{
    public UnityEvent<AbilityType> onPerformed;

    public Ability()
    {
        onPerformed = new UnityEvent<AbilityType>();
    }
}
