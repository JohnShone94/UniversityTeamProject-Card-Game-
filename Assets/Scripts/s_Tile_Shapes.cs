using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_Tile_Shapes : MonoBehaviour
{
    public s_Card_Type Card { get; set; }
    public int column { get; set; }
    public int row { get; set; }

    public string type { get; set; }

    public s_Tile_Shapes()
    {
        Card = s_Card_Type.None;
    }

    public bool isSameType(s_Tile_Shapes otherShape)
    {
        if(otherShape == null || !(otherShape is s_Tile_Shapes))
        {
            throw new ArgumentException("otherShape");
        }

        return string.Compare(this.type, (otherShape as s_Tile_Shapes).type) == 0;
    }

    public void assignCard(string setType, int setRow, int setColumn)
    {
        if(string.IsNullOrEmpty(type))
        {
            throw new ArgumentException("type");
        }

        column = setColumn;
        row = setRow;
        type = setType;
    }

    public static void changeColumnRow(s_Tile_Shapes a, s_Tile_Shapes b)
    {
        int tempRow = a.row;
        a.row = b.row;
        b.row = tempRow;

        int tempColumn = a.column;
        a.column = b.column;
        b.column = tempColumn;

    }

}

