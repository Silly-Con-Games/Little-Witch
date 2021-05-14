using UnityEngine;
using UnityEditor;

/// <summary>
/// An editor script which sets all the materials in the current scene to use the custom Toon shader.
/// </summary>
public class SetBiomeTypes
{    
    [MenuItem("Utils/Set Biomes to tiles")]
    private static void SetMaterials()
    {
        Tile[] allTiles = Object.FindObjectsOfType<Tile>();

        foreach (var tile in allTiles)
        {
			if (tile.WantsToBeSet()) { 
				tile.Setup();
			}
        }
    }

    [MenuItem("Utils/Revive selected tiles")]
    private static void ReviveTiles()
    {
        foreach (var tile in Selection.gameObjects)
        {
            tile.GetComponent<Tile>()?.Revive();               
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
