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

    private bool initialized;

    void Awake() {
		initialized = false;
	}

	void Start() {
        if (!initialized) {
			Initialize();
		}
    }

	private void Initialize() {
		tileMask = LayerMask.GetMask("Tile");

		initTile = FindObjectOfType<Tile>();

		aliveTilesCnt = 0;
		Queue<Tile> tilesQueue = new Queue<Tile>();
		tilesQueue.Enqueue(initTile);
		initTile.mapInfo.visited = true;
		Tile tile;
		tiles = new List<Tile>();

		while (tilesQueue.Count > 0) {
			tile = tilesQueue.Dequeue();
			tile.mapInfo.index = tiles.Count;
			tiles.Add(tile);
			if (tile.GetBiomeType() != BiomeType.DEAD) {
				aliveTilesCnt++;
			}

			foreach (Tile ngb in tile.GetNeighbours()) {
				if (ngb && !(ngb.mapInfo.visited)) {
					ngb.mapInfo.visited = true;
					tilesQueue.Enqueue(ngb);
				}
			}
		}

		initialized = true;
	}

	public void SetTiles(List<TileSaveInfo> savedTiles) {
		if (!initialized) {
			Initialize();
		}

		for (int i = 0; i < tiles.Count; i++) {
			if (tiles[i].GetBiomeType() != savedTiles[i].type) {
				tiles[i].Morph(savedTiles[i].type, true);
				tiles[i].SetupPropOnLoad(savedTiles[i].hasProp);
            }
		}

        // restoring alive tiles count
        int cnt = 0;
        for (int i = 0; i < tiles.Count; i++)
        {
            if (!(tiles[i].GetBiomeType() == BiomeType.DEAD))
            {
                cnt++;
            }
        }
        aliveTilesCnt = cnt;
    }

	public List<TileSaveInfo> GetTiles() {
		List<TileSaveInfo> tilesInfo = new List<TileSaveInfo>();
		for (int i = 0; i < tiles.Count; i++) {
			TileSaveInfo info = new TileSaveInfo();
			info.type = tiles[i].GetBiomeType();
			info.hasProp = tiles[i].HasProp();
			tilesInfo.Add(info);
		}

		return tilesInfo;
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

	public void SetPlayerPosition(Vector3 playerPosition) 
	{
		Tile playerTile = GetTileAtPosition(playerPosition);
        if (playerTile == null)
            return;
		playerTile.SetGrassPlayerPosition(playerPosition);

		foreach (Tile ngb in playerTile.GetNeighbours()) 
		{
			ngb?.SetGrassPlayerPosition(playerPosition);
		}
	}

    public void AttackTile(Tile tile)
    {
        aliveTilesCnt--;
        tile.Morph(BiomeType.DEAD, false);
    }

	public void ReviveTile() 
	{ 
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
			{ 
                cachedTile = hit.transform.gameObject.GetComponent<Tile>();
			}
            else
			{
                cachedTile = null;
			}
        }

        return cachedTile;
    }
}
