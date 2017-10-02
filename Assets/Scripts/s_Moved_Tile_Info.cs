using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class s_moved_Tile_Info
{
    private List<GameObject> newTile { get; set; }
    public int maxMoveDistance { get; set; }

    public IEnumerable<GameObject> movedTile
    {
        get
        {
            return newTile.Distinct();
        }
    }

    public void addTile(GameObject tile)
    {
        if(!newTile.Contains(tile))
        {
            newTile.Add(tile);
        }
    }

    public s_moved_Tile_Info()
    {
        newTile = new List<GameObject>();
    }
}
