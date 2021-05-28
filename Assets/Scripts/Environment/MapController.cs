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

            foreach (Tile ngb in tile.GetNeighbours())
            {
                if (ngb && !(ngb.mapInfo.visited))
                {
					ngb.mapInfo.visited = true;
                    tilesQueue.Enqueue(ngb);
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

	public void ReviveTile(Tile tile) 
	{ 
		tiles[tile.mapInfo.index] = tile;
		aliveTilesCnt++;
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

    public float TileHeightInPosition(Vector3 position)
    {
        Tile tile = GetTileAtPosition(position);
        if (tile != null)
        {
            return tile.transform.position.y;
        }

        return float.NaN;
    }


    private Tile cachedTile;
    private Vector3 lastPos = Vector3.positiveInfinity;
    public Tile GetTileAtPosition(Vector3 position)
    {
        if (lastPos != position)
        {
            lastPos = position;
            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 3f, tileMask))
                cachedTile = hit.transform.gameObject.GetComponent<Tile>();
            else
                cachedTile = null;
        }
        return cachedTile;
    }
}
