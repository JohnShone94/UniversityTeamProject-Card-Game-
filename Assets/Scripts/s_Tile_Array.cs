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
}


///https://dgkanatsios.com/2015/02/25/building-a-match-3-game-in-unity-3/