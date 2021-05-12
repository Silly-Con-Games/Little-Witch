using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorsCreator : MonoBehaviour
{

    [SerializeField]
    private GameObject indicatorUIPrefab;

    public RectTransform holder;

    public GameObject CreateIndicator()
    {
        GameObject indicator = Instantiate(indicatorUIPrefab, holder);
        return indicator;
    }
    
}
