using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class IndicatorsCreator : MonoBehaviour
{

    [SerializeField]
    private GameObject indicatorUIPrefab;

    public RectTransform holder;


    private static IndicatorsCreator instance;

    public void Awake()
    {
        Assert.IsTrue(instance == null);
        instance = this;
    }

    public static GameObject CreateIndicator()
    { 
        return Instantiate(instance.indicatorUIPrefab, instance.holder); 
    }
    
}
