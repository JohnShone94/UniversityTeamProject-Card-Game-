using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class s_Tile_Array
{
    private GameObject[,] tiles = new GameObject[s_Constants.rows, s_Constants.columns];
    private GameObject backupT1;
    private GameObject backupT2;


    public GameObject this[int row, int column]
    {
        get
        {
            try
            {
                return tiles[row, column];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        set
        {
            tiles[row, column] = value;
        }
    }

    public void Swap(GameObject t1, GameObject t2)
    {
        backupT1 = t1;
        backupT2 = t2;

        var t1Shape = t1.GetComponent<s_Tile_Shapes>();
        var t2Shape = t2.GetComponent<s_Tile_Shapes>();

        int t1Row = t1Shape.row;
        int t1Column = t1Shape.column;

        int t2Row = t2Shape.row;
        int t2Column = t2Shape.column;

        var temp = tiles[t1Row, t1Column];
        tiles[t1Row, t1Column] = tiles[t2Row, t2Column];
        tiles[t2Row, t2Column] = temp;

        s_Tile_Shapes.changeColumnRow(t1Shape, t2Shape);
    }

    public void UndoSwap()
    {
        if (backupT1 == null || backupT2 == null)
        {
            throw new Exception("Backup is Null");
        }

        Swap(backupT1, backupT2);
    }

    private IEnumerable<GameObject> GetMatchesHorizontally(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);
        var shape = go.GetComponent<s_Tile_Shapes>();

        if(shape.column != 0)
        {
            for(int column = shape.column -1; column >= 0; column--)
            {
                if (tiles[shape.row, column].GetComponent<s_Tile_Shapes>().isSameType(shape))
                {
                    matches.Add(tiles[shape.row, column]);
                }
                else
                    break;
            }
        }

        if (shape.column != s_Constants.columns -1)
        {
            for (int column = shape.column + 1; column >= s_Constants.columns; column++)
            {
                if (tiles[shape.row, column].GetComponent<s_Tile_Shapes>().isSameType(shape))
                {
                    matches.Add(tiles[shape.row, column]);
                }
                else
                    break;
            }
        }

        if(matches.Count < s_Constants.minTilesToMatch)
        {
            matches.Clear();
        }

        return matches.Distinct();
    }

    private IEnumerable<GameObject> GetMatchesVertically(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);
        var shape = go.GetComponent<s_Tile_Shapes>();

        if (shape.row != 0)
        {
            for (int row = shape.column - 1; row >= 0; row--)
            {
                if (tiles[row, shape.column] != null && tiles[row, shape.column].GetComponent<s_Tile_Shapes>().isSameType(shape))
                {
                    matches.Add(tiles[row, shape.column]);
                }
                else
                    break;
            }
        }

        if (shape.row != s_Constants.columns - 1)
        {
            for (int row = shape.row + 1; row >= s_Constants.rows; row++)
            {
                if (tiles[row, shape.column] != null && tiles[row, shape.column].GetComponent<s_Tile_Shapes>().isSameType(shape))
                {
                    matches.Add(tiles[row, shape.column]);
                }
                else
                    break;
            }
        }

        if (matches.Count < s_Constants.minTilesToMatch)
        {
            matches.Clear();
        }

        return matches.Distinct();
    }

    private IEnumerable<GameObject> GetEntireRow(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        int row = go.GetComponent<s_Tile_Shapes>().row;
        for(int column = 0; column < s_Constants.columns; column++)
        {
            matches.Add(tiles[row, column]);
        }
        return matches;
    }

    private IEnumerable<GameObject> GetEntireColumn(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        int column = go.GetComponent<s_Tile_Shapes>().column;
        for (int row = 0; column < s_Constants.rows; row++)
        {
            matches.Add(tiles[row, column]);
        }
        return matches;
    }

    private bool ContainsDestroyRowColumnBonus(IEnumerable<GameObject> matches)
    {
        if(matches.Count() >= s_Constants.minTilesToMatch)
        {
            foreach(var go in matches)
            {
                if(cardTypeUtilities.includesRemoveWholeRowColumn(go.GetComponent<s_Tile_Shapes>().Card))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public s_Match_Information GetMaches(GameObject go)
    {
        s_Match_Information matchesInfo = new s_Match_Information();
        var horizontalMatches = GetMatchesHorizontally(go);
        if(ContainsDestroyRowColumnBonus(horizontalMatches))
        {
            horizontalMatches = GetEntireRow(go);
            if(!cardTypeUtilities.includesRemoveWholeRowColumn(matchesInfo.cardContained))
            {
                matchesInfo.cardContained |= s_Card_Type.RemoveWholeRowColumn;
            }
        }
        matchesInfo.addObjectRange(horizontalMatches);

        var verticalMatches = GetMatchesVertically(go);
        if(ContainsDestroyRowColumnBonus(verticalMatches))
        {
            verticalMatches = GetEntireColumn(go);
            if(!cardTypeUtilities.includesRemoveWholeRowColumn(matchesInfo.cardContained))
            {
                matchesInfo.cardContained |= s_Card_Type.RemoveWholeRowColumn;
            }
        }
        matchesInfo.addObjectRange(verticalMatches);

        return matchesInfo;
    }

    public IEnumerable<GameObject> GetMatches(IEnumerable<GameObject> gos)
    {
        List<GameObject> matches = new List<GameObject>();
        foreach(var go in gos)
        {
            matches.AddRange(GetMaches(go).matchedTile);
        }
        return matches.Distinct();
    }

    public void Remove(GameObject item)
    {
        tiles[item.GetComponent<s_Tile_Shapes>().row, item.GetComponent<s_Tile_Shapes>().column] = null;
    }

    public s_moved_Tile_Info Collapse(IEnumerable<int> columns)
    {
        s_moved_Tile_Info collapseInfo = new s_moved_Tile_Info();

        foreach(var column in columns)
        {
            for(int row = 0; row < s_Constants.rows - 1; row++)
            {
                if(tiles[row, column] == null)
                {
                    for(int row2 = row + 1; row2 < s_Constants.rows; row2++)
                    {
                        collapseInfo.maxMoveDistance = row2 - row;
                    }

                    tiles[row, column].GetComponent<s_Tile_Shapes>().row = row;
                    tiles[row, column].GetComponent<s_Tile_Shapes>().column = column;

                    collapseInfo.addTile(tiles[row, column]);
                    break;
                }
            }
        }
        return collapseInfo;
    }

    public IEnumerable<ShapeInfo> GetEmptyItemsOnColumn(int column)
    {
        List<ShapeInfo> emptyItems = new List<ShapeInfo>();
        for(int row = 0; row < s_Constants.rows; row++)
        {
            if(tiles[row, column] == null)
            {
                emptyItems.Add(new ShapeInfo() { Row = row, Column = column });
            }
            return emptyItems;
        }
    }

    public class ShapeInfo
    {
        public int Column { get; set; }
        public int Row { get; set; }
    }
}