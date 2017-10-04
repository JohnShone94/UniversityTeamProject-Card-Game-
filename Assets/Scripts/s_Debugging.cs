using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class s_Debugging : MonoBehaviour
{
    public static string[,] Fill_Tiles_Array()
    {
        string[,] tiles = new string[s_Constants.rows, s_Constants.columns];
        TextAsset txt = Resources.Load("Level") as TextAsset;
        string level = txt.text;

        string[] lines = level.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        for(int row = s_Constants.rows - 1; row >= 0; row--)
        {
            string[] items = lines[row].Split('|');
            for(int column = 0; column < s_Constants.columns; column++)
            {
                tiles[row, column] = items[column];
            }
        }
        return tiles;
    }

    public static string Get_Array_Contents(s_TileArray tiles)
    {
        string empty = string.Empty;
        for (int row = s_Constants.rows - 1; row >= 0; row--)
        {

            for (int column = 0; column < s_Constants.columns; column++)
            {
                if (tiles[row, column] == null)
                {
                    empty += "NULL  |";
                }
                else
                {
                    var tile = tiles[row, column].GetComponent<s_Tiles>();
                    empty += tile.row.ToString("D2") + "-" + tile.column.ToString("D2");
                    empty += tile.Type.Substring(5, 2);

                    if (s_Card_Utilities.Includes_Remove_Whole_Row_Column(tile.Card))
                    {
                        empty += "B";
                    }
                    else
                    {
                        empty += " ";
                    }
                    empty += " | ";
                }
            }
            empty += Environment.NewLine;
        }
        return empty;
    }
}
