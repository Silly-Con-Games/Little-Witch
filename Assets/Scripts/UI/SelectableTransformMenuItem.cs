using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableTransformMenuItem : Selectable
{
    [SerializeField] private BiomeType biome;

    [SerializeField] private Image bg;

    [SerializeField] private GameObject active;
    [SerializeField] private Image inactive;

    private TransformMenu transformMenu;

    void Start()
    {
        inactive.alphaHitTestMinimumThreshold = 0.3f;
        transformMenu = GetComponentInParent<TransformMenu>();
        DeselectThis();
    }

    //Detect if the Cursor starts to pass over the GameObject
    public override void OnPointerEnter(PointerEventData pointerEventData)
    {
        SelectThis();
    }

    //Detect when Cursor leaves the GameObject
    public override void OnPointerExit(PointerEventData pointerEventData)
    {
        DeselectThis();
    }

    private void SelectThis()
    {
        inactive.enabled = false;
        active.SetActive(true);
        //bg.enabled = true;
        transformMenu.Select(biome);
    }

    private void DeselectThis()
    {
        inactive.enabled = true;
        active.SetActive(false);
        //bg.enabled = false;
        transformMenu.Select(BiomeType.UNKNOWN);
    }

    protected override void OnDisable()
    {
        DeselectThis();
    }


}
