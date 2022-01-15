using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialLayoutGroup : MonoBehaviour
{
    private RectTransform[] children;

    void Start()
    {
        children = GetComponentsInChildren<RectTransform>();
    }


    private void Arrange()
    {
        for (int i = 0; i < children.Length; i++)
        {

        }
    }
}
