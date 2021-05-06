using UnityEngine;
using UnityEditor;

/// <summary>
/// An editor script which sets all the materials in the current scene to use the custom Toon shader.
/// </summary>
public class SetShader
{    
    [MenuItem("Utils/Set Materials to Toon Shader")]
    private static void SetMaterials()
    {
        Shader toonShader = Shader.Find("Shader Graphs/Toon");

        MeshRenderer[] allRenderers = Object.FindObjectsOfType<MeshRenderer>();

        foreach (var renderer in allRenderers)
        {
            Material[] materials = renderer.sharedMaterials;
            foreach (var material in materials)
            {
                material.shader = toonShader;
                material.SetColor("_SpecularColor", Color.black);
            }
        }
    }
}
