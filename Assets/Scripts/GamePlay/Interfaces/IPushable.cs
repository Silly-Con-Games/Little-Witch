using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPushable : IObjectType
{
    void ReceivePush(Vector3 force, float duration);
}
