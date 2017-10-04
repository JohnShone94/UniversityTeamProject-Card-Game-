using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class s_MovedTileInfo
{
    private List<GameObject> newTile { get; set; }
    public int maxMoveDistance { get; set; }

    public IEnumerable<GameObject> Moved_Tile
    {
        get
        {
            return newTile.Distinct();
        }
    }

    public void Add_Tile(GameObject tile)
    {
        if(!newTile.Contains(tile))
        {
            newTile.Add(tile);
        }
    }

    public s_MovedTileInfo()
    {
        newTile = new List<GameObject>();
    }
}
