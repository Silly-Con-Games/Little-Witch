using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class TileMap : MonoBehaviour
{
    public int width;
    public int height;

    public Tile tilePrefab;
    public bool ShouldRegenerate = false;
    public float noiseScale = 0.5f;


    public void OnValidate()
    {
#if UNITY_EDITOR
        if (ShouldRegenerate)
        {
            int newNoise = Random.Range(0, 10000);

            EditorApplication.delayCall += () =>
                {
                    var tempList = transform.Cast<Transform>().ToList();
                    foreach (var child in tempList)
                    {
                        DestroyImmediate(child.gameObject);
                    }

                    EditorApplication.delayCall += () =>
                    {
                        for (int x = 0; x < width; x++)
                        {
                            for (int y = 0; y < height; y++)
                            {
                                //var inst = (PrefabUtility.InstantiatePrefab(tilePrefab, transform) as Tile).transform;
                                var inst = Instantiate(tilePrefab, transform);
                                float offset = y % 2 == 0 ? 0.8660254f : 0;
                                inst.transform.position = new Vector3(offset + 0.8660254f * 2 * (x - width / 2), 0, 1.5f * (y - height / 2));
                                float val = Mathf.PerlinNoise(x* noiseScale + newNoise, y* noiseScale + newNoise);

                                if(val < 0.44f)
                                    inst.Morph(BiomeType.FOREST, true);
                                else if(val < 0.68f)
                                    inst.Morph(BiomeType.MEADOW, true);
                                else 
                                    inst.Morph(BiomeType.WATER, true);
                            }
                        }
                    };
                };
        }
#endif
    }
}
