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

    // forbid this if player doesn't have energy to transform !
    public void OpenMenu(bool canOpen)
    {
        if (!canOpen) return;

        SlowGame();
        contents.SetActive(true);
        playerController.canBeControlled = false;
    }

    public void CloseMenu()
    {
        ResumeGame();
        if (playerController)
        {
            playerController.transformAbility.StopHighlightTransform();
            playerController.canBeControlled = true;
            if (selected != BiomeType.UNKNOWN)
            {
                playerController.Transform(selected);
            }
            
        }
        contents.SetActive(false);

    }

    public void Select(BiomeType biome)
    {
        selected = biome;
        if (playerController)
        {
            playerController.transformAbility.StopHighlightTransform();

            // Highighting needs to be updated if 1. player gets energy during slowmo 2. moves to different tile 3. enemy kills a tile
            playerController.transformAbility.HighlightTransform(biome);
        }
        
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
