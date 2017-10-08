using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class s_Tiles : MonoBehaviour
{
    public s_CardType Card { get; set; }
    public int column { get; set; }
    public int row { get; set; }

    public string Type { get; set; }

    public s_Tiles()
    {
        Card = s_CardType.None;
    }

    public bool Is_Same_Type(s_Tiles otherShape)
    {
        if(otherShape == null || !(otherShape is s_Tiles))
        {
            throw new ArgumentException("otherShape");
        }

        return string.Compare(this.Type, (otherShape as s_Tiles).Type) == 0;
    }

   public void Assign_Card(string setType, int setRow, int setColumn)
    {
        if(string.IsNullOrEmpty(setType))
        {
            throw new ArgumentException("type");
        }

        column = setColumn;
        row = setRow;
        Type = setType;
    }

    public static void Change_Column_Row(s_Tiles a, s_Tiles b)
    {
        int tempRow = a.row;
        a.row = b.row;
        b.row = tempRow;

        int tempColumn = a.column;
        a.column = b.column;
        b.column = tempColumn;

    }

}

