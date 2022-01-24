using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlighter : MonoBehaviour
{
    public SpriteRenderer s;

    public void SetHighlightColor(Color color)
    {
        s.color = color;
    }
}
