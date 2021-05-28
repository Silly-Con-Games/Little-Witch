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

    public bool KillDeadBefore = false;
    public float reviveCircleInMiddle = -1; 

    [Tooltip("Read only")]
    public int currentSeed;

    public bool useCustom = false;
    [Tooltip("Custom seed")]
    public int customSeed;

    public Transform parent;

    public void OnValidate()
    {
#if UNITY_EDITOR
        if (ShouldRegenerate)
        {
            ShouldRegenerate = false;
            if (useCustom)
                currentSeed = customSeed;
            else
                currentSeed = (int)(System.DateTime.UtcNow.Ticks* System.DateTime.UtcNow.Ticks* System.DateTime.UtcNow.Ticks);
            Random.InitState(currentSeed);

            int newNoise = Random.Range(0, 50000);
          
            EditorApplication.delayCall += () =>
                {
                    HashSet<Vector2> dead = new HashSet<Vector2>();
                    var tempList = parent.Cast<Transform>().ToList();
                    foreach (var child in tempList)
                    {
                        var tile = child.gameObject.GetComponent<Tile>();
                        if(tile != null && tile.GetBiomeType() == BiomeType.DEAD) 
                        {
                            Vector2 v = new Vector2(child.position.x, child.position.z);
                            dead.Add(child.position);
                        }
                        DestroyImmediate(child.gameObject);
                    }

                    EditorApplication.delayCall += () =>
                    {
                        for (int x = 0; x < width; x++)
                        {
                            for (int y = 0; y < height; y++)
                            {
                                //var inst = (PrefabUtility.InstantiatePrefab(tilePrefab, transform) as Tile).transform;
                                var inst = Instantiate(tilePrefab, parent);
                                float offset = y % 2 == 0 ? 0.8660254f : 0;
                                inst.transform.position = new Vector3(offset + 0.8660254f * 2 * (x - width / 2), 0, 1.5f * (y - height / 2));
                                float val = Mathf.PerlinNoise(x* noiseScale + newNoise, y* noiseScale + newNoise);

                                if(val < 0.44f)
                                    inst.Morph(BiomeType.FOREST, true);
                                else if(val < 0.68f)
                                    inst.Morph(BiomeType.MEADOW, true);
                                else 
                                    inst.Morph(BiomeType.WATER, true);
                                Vector2 v = new Vector2(inst.transform.position.x, inst.transform.position.z);

                                if (reviveCircleInMiddle > 0 && v.sqrMagnitude > reviveCircleInMiddle * reviveCircleInMiddle)
                                {
                                    inst.Kill();
                                }

                                if (KillDeadBefore && dead.Contains(v))
                                    inst.Kill();
                            }
                        }
                    };
                };
        }
#endif
    }
}
