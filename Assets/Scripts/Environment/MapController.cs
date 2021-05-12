using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PropAndProbability
{
    [Tooltip("Has to implement IProp interface")]
    public GameObject prop;
    public float chance;
}

public class MapController : MonoBehaviour
{
    private Tile initTile;

    public PropAndProbability meadowProp;
    public PropAndProbability forestProp;
    public PropAndProbability waterProp;

    public List<Tile> tiles { get; set; }

    public int aliveTilesCnt { get; set; }

    private int tileMask;

    // Start is called before the first frame update
    void Start()
    {
        tileMask = LayerMask.GetMask("Tile");

        initTile = FindObjectOfType<Tile>();

        aliveTilesCnt = 0;
        Queue<Tile> tilesQueue = new Queue<Tile>();
        tilesQueue.Enqueue(initTile);
        initTile.mapInfo.visited = true;
        Tile tile;
        tiles = new List<Tile>();

        while (tilesQueue.Count > 0)
        {
            tile = tilesQueue.Dequeue();
            tile.mapInfo.index = tiles.Count;
            if (tile.GetBiomeType() != BiomeType.DEAD)
            {
                tiles.Add(tile);
                aliveTilesCnt++;
            }
            else
            {
                tiles.Add(null);
            }
            for (int i = 0; i < tile.GetNeighbours().Length; i++)
            {
                if (tile.GetNeighbour(i) && !(tile.GetNeighbour(i).mapInfo.visited))
                {
                    tile.GetNeighbour(i).mapInfo.visited = true;
                    tilesQueue.Enqueue(tile.GetNeighbour(i));
                }
            }
        }
    }

    public PropAndProbability GetProp(BiomeType type)
    {
        switch (type)
        {
            case BiomeType.FOREST:
                return forestProp;
            case BiomeType.MEADOW:
                return meadowProp;
            case BiomeType.WATER:
                return waterProp;
            default:
                return null;
        }
    }

    public void AttackTile(Tile tile)
    {
        tiles[tile.mapInfo.index] = null;
        aliveTilesCnt--;
        tile.Morph(BiomeType.DEAD, false);
    }


    public BiomeType BiomeTypeInPosition(Vector3 position)
    {
        Tile tile = GetTileAtPosition(position);
        if (tile != null)
        {
            return tile.GetBiomeType();
        }

        return BiomeType.UNKNOWN;
    }

    public bool Transform(Vector3 atPosition, BiomeType target)
    {
        Tile tile = GetTileAtPosition(atPosition);

        if(tile != null)
        {
            tile.Morph(target, false);
            foreach (Tile ngb in tile.GetNeighbours())
            {
                if (ngb != null)
                {
                    ngb.Morph(target, false);
                }
            }
            return true;
        }                 
        return false;
    }

    private Tile cachedTile;
    private Vector3 lastPos;
    public Tile GetTileAtPosition(Vector3 position)
    {
        if (lastPos != position)
        {
            lastPos = position;
            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 2, tileMask))
                cachedTile = hit.transform.gameObject.GetComponent<Tile>();
            else
                cachedTile = null;
        }
        return cachedTile;
    }
}
