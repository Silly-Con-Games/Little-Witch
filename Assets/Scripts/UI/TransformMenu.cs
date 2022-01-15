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
        SlowGame();
        contents.SetActive(true);
    }

    public void CloseMenu()
    {
        ResumeGame();
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

    private void SlowGame()
    {
        Time.timeScale = .1f;
        StartCoroutine(SpeedUp());
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    private IEnumerator SpeedUp()
    {
        while (Time.timeScale < 1)
        {
            Time.timeScale += .05f;
            yield return new WaitForSeconds(.1f);
        }
        ResumeGame();
    }
}
