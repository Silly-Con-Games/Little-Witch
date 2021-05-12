using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class TileMap : MonoBehaviour
{
    public int width;
    public int height;

    public Tile tilePrefab;

    public void OnValidate()
    {
        UnityEditor.EditorApplication.delayCall += () =>
        {
            var tempList = transform.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                DestroyImmediate(child.gameObject);
            }

            UnityEditor.EditorApplication.delayCall += () =>
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var inst = Instantiate(tilePrefab, transform).transform;
                        float offset = y % 2 == 0 ? 0.8660254f : 0;
                        inst.position = new Vector3(offset + 0.8660254f * 2 * (x - width / 2), 0, 1.5f * (y - height / 2));
                    }
                }
            };
        };              
    }
}
