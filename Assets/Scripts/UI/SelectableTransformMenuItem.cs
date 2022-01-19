using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableTransformMenuItem : Selectable
{
    [SerializeField] private BiomeType biome;

    private TransformMenu transformMenu;

    new void Start()
    {
        //Debug.Log("in start of selectable " + name);
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.3f;
        transformMenu = GetComponentInParent<TransformMenu>();
        //DeselectThis();
    }

/*    new void OnSelect(BaseEventData data)
    {
        Debug.Log("Selecting " + name);
        transformMenu.Select(biome);
    }

    new void OnDeselect(BaseEventData data)
    {
        Debug.Log("Deselecting " + name);
        transformMenu.Select(BiomeType.UNKNOWN);
    }*/

    protected new void OnDisable()
    {
        //DeselectThis();
    }


}
