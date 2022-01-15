using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransformMenuItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private BiomeType biome;

    private Image bg;
    private TransformMenu tm;

    void Start()
    {
        bg = GetComponent<Image>();
        tm = GetComponentInParent<TransformMenu>();
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        Select();
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Deselect();
    }

    private void Select()
    {
        bg.enabled = true;
        tm.Select(biome);
    }

    private void Deselect()
    {
        bg.enabled = false;
        tm.Select(BiomeType.UNKNOWN);
    }

    private void OnDisable()
    {
        Deselect(); 
    }

}
