using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class s_MatchInformation
{
    private List<GameObject> matchedTiles;

    public IEnumerable<GameObject> Matched_Tiles
    {
        get
        {
            return matchedTiles.Distinct();
        }
    }

    public void Add_Matched_Tile(GameObject tile)
    {
        if(!matchedTiles.Contains(tile))
        {
            matchedTiles.Add(tile);
        }
    }

    public void Add_Matched_Tiles_Range(IEnumerable<GameObject> tiles)
    {
        foreach (var item in tiles)
        {
            Add_Matched_Tile(item);
        }
    }

    public s_MatchInformation()
    {
        matchedTiles = new List<GameObject>();
        Card_Contained = s_CardType.None;
    }

    public s_CardType Card_Contained { get; set; }
}
