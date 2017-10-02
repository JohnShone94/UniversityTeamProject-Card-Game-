using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class s_Match_Information
{
    private List<GameObject> matchedTiles;

    public IEnumerable<GameObject> matchedTile
    {
        get
        {
            return matchedTiles.Distinct();
        }
    }

    public void addObject(GameObject go)
    {
        if(!matchedTiles.Contains(go))
        {
            matchedTiles.Add(go);
        }
    }

    public void addObjectRange(IEnumerable<GameObject> gos)
    {
        foreach (var item in gos)
        {
            addObject(item);
        }
    }

    public s_Match_Information()
    {
        matchedTiles = new List<GameObject>();
        cardContained = s_Card_Type.None;
    }

    public s_Card_Type cardContained { get; set; }
}
