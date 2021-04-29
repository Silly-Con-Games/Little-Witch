using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{

    [SerializeField]
    private Tile initTile;
    
    public List<Tile> tiles { get; set; }

    public int aliveTilesCnt { get; set; }
    
    // Start is called before the first frame update
    void Start()
    {

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
            if (tile.tileState == TileState.ALIVE || tile.tileState == TileState.CHANGING)
            {
                tiles.Add(tile);
                aliveTilesCnt++;
            }
            else
            {
                tiles.Add(null);
            }
            for (int i = 0; i < tile.neighbours.Length; i++)
            {
                if (tile.neighbours[i] && !(tile.neighbours[i].mapInfo.visited))
                {
                    tile.neighbours[i].mapInfo.visited = true;
                    tilesQueue.Enqueue(tile.neighbours[i]);
                }
            }
        }
    }
    
    public void AttackTile(Tile tile)
    {
        tiles[tile.mapInfo.index] = null;
        aliveTilesCnt--;
        tile.Die(false);
    }


    public BiomeType BiomeTypeInPosition(Vector3 position)
    {
        if (Physics.Raycast(position, new Vector3(0,-1, 0.01f), out RaycastHit hit, 10f, LayerMask.GetMask("Tile"))) 
        {
            Tile tile = hit.transform.parent?.gameObject.GetComponent<Tile>();
            if(tile != null)
            {
                return ExtractBiomeTypeFromTile(tile);
            }
        }

      return BiomeType.unknown;
    }

    private BiomeType ExtractBiomeTypeFromTile(Tile tile)
    {
        if (tile.isDead)
            return BiomeType.dead;

        switch (tile.tileType)
        {
            case TileType.FOREST:
                return BiomeType.forest;
            case TileType.PLAIN:
                return BiomeType.meadow;
            case TileType.WATER:
                return BiomeType.water;
            default:
                return BiomeType.unknown;
        }
    }
}
