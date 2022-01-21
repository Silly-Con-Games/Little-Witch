using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// add controller support - make items Selectable ?
public class TransformMenuItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        Deselect();
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
        FMODUnity.RuntimeManager.PlayOneShot("event:/menu_click");
        inactive.enabled = false;
        active.SetActive(true);
        //bg.enabled = true;
        transformMenu.Select(biome);
    }

    private void Deselect()
    {
        inactive.enabled = true;
        active.SetActive(false);
        //bg.enabled = false;
        transformMenu.Select(BiomeType.UNKNOWN);
    }

    private void OnDisable()
    {
        Deselect(); 
    }

}
