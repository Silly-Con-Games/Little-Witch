using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TransformMenu : MonoBehaviour
{
    [SerializeField] private GameObject contents;

    private BiomeType selected = BiomeType.UNKNOWN;

    public PlayerController playerController;

    //private SelectableTransformMenuItem firstItem;

    public void Start()
    {
        //firstItem = GetComponentInChildren<SelectableTransformMenuItem>();
        CloseMenu();
    }

    // forbid this if player doesn't have energy to transform !
    public void OpenMenu(bool canOpen)
    {
        if (!canOpen) return;

        SlowGame();
        contents.SetActive(true);
        playerController.canBeControlled = false;
        //if (playerController.gamepadActive) firstItem.Select();
    }

    public void CloseMenu()
    {
        if (playerController) playerController.canBeControlled = true;

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

    // u should also forbid movement, dash, use of other abilities etc.
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
