using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_TileShapes : MonoBehaviour
{
    public s_CardType Card { get; set; }
    public int column { get; set; }
    public int row { get; set; }

    public string type { get; set; }

    public s_TileShapes()
    {
        Card = s_CardType.None;
    }

    public bool Is_Same_Type(s_TileShapes otherShape)
    {
        if(otherShape == null || !(otherShape is s_TileShapes))
        {
            throw new ArgumentException("otherShape");
        }

        return string.Compare(this.type, (otherShape as s_TileShapes).type) == 0;
    }

    public void Assign_Card(string setType, int setRow, int setColumn)
    {
        if(string.IsNullOrEmpty(type))
        {
            throw new ArgumentException("type");
        }

        column = setColumn;
        row = setRow;
        type = setType;
    }

    public static void Change_Column_Row(s_TileShapes a, s_TileShapes b)
    {
        int tempRow = a.row;
        a.row = b.row;
        b.row = tempRow;

        int tempColumn = a.column;
        a.column = b.column;
        b.column = tempColumn;

    }

}

