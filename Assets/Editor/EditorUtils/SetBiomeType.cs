using UnityEngine;
using UnityEditor;

/// <summary>
/// An editor script which sets all the materials in the current scene to use the custom Toon shader.
/// </summary>
public class SetBiomeTypes
{    
    [MenuItem("Utils/Set Biomes to all tiles")]
    private static void SetAllMaterials()
    {
        Tile[] allTiles = Object.FindObjectsOfType<Tile>();

        foreach (var tile in allTiles)
        {
			if (tile.WantsToBeSet()) { 
				tile.Setup();
			}
        }
    }

    [MenuItem("Utils/Set Biomes to tiles")]
    private static void SetMaterials()
    {
        foreach (var tile in Selection.gameObjects)
        {
            var t = tile.GetComponent<Tile>();
            if (t!= null && t.WantsToBeSet())
            {
                t.Setup();
            }
        }
    }

    [MenuItem("Utils/Revive all tiles")]
    private static void ReviveAllTiles()
    {
        Tile[] allTiles = Object.FindObjectsOfType<Tile>();

        foreach (var tile in allTiles)
        {
            tile.ReviveInEditor();
        }
    }

    [MenuItem("Utils/Kill all tiles")]
    private static void KillAllTiles()
    {
        Tile[] allTiles = Object.FindObjectsOfType<Tile>();

        foreach (var tile in allTiles)
        {
            tile.Kill();
        }
    }

    [MenuItem("Utils/Revive selected tiles")]
    private static void ReviveTiles()
    {
        foreach (var tile in Selection.gameObjects)
        {
            tile.GetComponent<Tile>()?.ReviveInEditor();               
        }
    }

    [MenuItem("Utils/Kill selected tiles")]
    private static void KillTiles()
    {
        foreach (var tile in Selection.gameObjects)
        {
            tile.GetComponent<Tile>()?.Kill();
        }
    }
}
