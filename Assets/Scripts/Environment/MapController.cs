using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    private Tile initTile;

    public List<Tile> tiles { get; set; }

    public int aliveTilesCnt { get; set; }

    // Start is called before the first frame update
    void Start()
    {
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

    public void AttackTile(Tile tile)
    {
        tiles[tile.mapInfo.index] = null;
        aliveTilesCnt--;
        tile.Morph(BiomeType.DEAD, false);
    }


    public BiomeType BiomeTypeInPosition(Vector3 position)
    {
        if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 2f, LayerMask.GetMask("Tile")))
        {
			Tile tile = hit.transform.gameObject.GetComponent<Tile>();

            if(tile != null)
            {
                return tile.GetBiomeType();
            }
        }

        return BiomeType.UNKNOWN;
    }
}
