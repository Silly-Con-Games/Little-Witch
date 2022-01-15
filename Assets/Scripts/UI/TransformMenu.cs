using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TransformMenu : MonoBehaviour
{
    [SerializeField] private GameObject contents;

    private BiomeType selected = BiomeType.UNKNOWN;

    public PlayerController playerController;

    public void Start()
    {
        CloseMenu();
    }

    public void OpenMenu()
    {
        contents.SetActive(true);
    }

    public void CloseMenu()
    {
        if (selected != BiomeType.UNKNOWN && playerController)
        {
            playerController.Transform(selected);
        }
        contents.SetActive(false);
    }

    public void Select(BiomeType biome)
    {
        selected = biome;
    }
}
