using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class s_TileArray
{
    private GameObject[,] tilesA = new GameObject[s_Constants.rows, s_Constants.columns];
    private GameObject backupTileOne;
    private GameObject backupTileTwo;


    public GameObject this[int row, int column]
    {
        get
        {
            try
            {
                return tilesA[row, column];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        set
        {
            tilesA[row, column] = value;
        }
    }

    public void Swap(GameObject tileOne, GameObject tileTwo)
    {
        backupTileOne = tileOne;
        backupTileTwo = tileTwo;

        var tileOneShape = tileOne.GetComponent<s_Tiles>();
        var tileTwoShape = tileTwo.GetComponent<s_Tiles>();

        int tileOneRow = tileOneShape.row;
        int tileOneColumn = tileOneShape.column;

        int tileTwoRow = tileTwoShape.row;
        int tileTwoColumn = tileTwoShape.column;

        var tempTile = tilesA[tileOneRow, tileOneColumn];
        tilesA[tileOneRow, tileOneColumn] = tilesA[tileTwoRow, tileTwoColumn];
        tilesA[tileTwoRow, tileTwoColumn] = tempTile;

        s_Tiles.Change_Column_Row(tileOneShape, tileTwoShape);
    }

    public void Undo_Swap()
    {
        if (backupTileOne == null || backupTileTwo == null)
        {
            throw new Exception("Backup is Null");
        }

        Swap(backupTileOne, backupTileTwo);
    }

    public IEnumerable<GameObject> Get_Matches(IEnumerable<GameObject> tiles)
    {
        List<GameObject> matches = new List<GameObject>();
        foreach (var tile in tiles)
        {
            matches.AddRange(Get_Matches(tile).Matched_Tiles);
        }
        return matches.Distinct();
    }
    public s_MatchInformation Get_Matches(GameObject tile)
    {
        s_MatchInformation matchesInfo = new s_MatchInformation();

        var horizontalMatches = Get_Matches_Horizontally(tile);

        if (Contains_Destroy_Row_Column_Bonus(horizontalMatches))
        {
            horizontalMatches = Get_Entire_Row(tile);
            if (!s_Card_Utilities.Includes_Remove_Whole_Row_Column(matchesInfo.Card_Contained))
            {
                matchesInfo.Card_Contained |= s_CardType.RemoveWholeRowColumn;
            }
        }
        matchesInfo.Add_Matched_Tiles_Range(horizontalMatches);

        var verticalMatches = Get_Matches_Vertically(tile);

        if (Contains_Destroy_Row_Column_Bonus(verticalMatches))
        {
            verticalMatches = Get_Entire_Column(tile);
            if (!s_Card_Utilities.Includes_Remove_Whole_Row_Column(matchesInfo.Card_Contained))
            {
                matchesInfo.Card_Contained |= s_CardType.RemoveWholeRowColumn;
            }
        }
        matchesInfo.Add_Matched_Tiles_Range(verticalMatches);

        return matchesInfo;
    }

    private bool Contains_Destroy_Row_Column_Bonus(IEnumerable<GameObject> matches)
    {
        if (matches.Count() >= s_Constants.minTilesToMatch)
        {
            foreach (var tile in matches)
            {
                if (s_Card_Utilities.Includes_Remove_Whole_Row_Column(tile.GetComponent<s_Tiles>().Card))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private IEnumerable<GameObject> Get_Entire_Row(GameObject tile)
    {
        List<GameObject> matches = new List<GameObject>();
        int row = tile.GetComponent<s_Tiles>().row;
        for (int column = 0; column < s_Constants.columns; column++)
        {
            matches.Add(this.tilesA[row, column]);
        }
        return matches;
    }

    private IEnumerable<GameObject> Get_Entire_Column(GameObject tile)
    {
        List<GameObject> matches = new List<GameObject>();
        int column = tile.GetComponent<s_Tiles>().column;
        for (int row = 0; column < s_Constants.rows; row++)
        {
            matches.Add(tilesA[row, column]);
        }
        return matches;
    }

    private IEnumerable<GameObject> Get_Matches_Horizontally(GameObject tile)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(tile);
        var tiles = tile.GetComponent<s_Tiles>();

        if (tiles.column != 0)
        {
            for (int column = tiles.column - 1; column >= 0; column--)
            {
                if(tilesA[tiles.row, column].GetComponent<s_Tiles>().Is_Same_Type(tiles))
                {
                    matches.Add(tilesA[tiles.row, column]);
                }
                else
                    break;
            }
        }

        if (tiles.column != s_Constants.columns - 1)
        {
            for (int column = tiles.column + 1; column >= s_Constants.columns; column++)
            {
                if (this.tilesA[tiles.row, column].GetComponent<s_Tiles>().Is_Same_Type(tiles))
                {
                    matches.Add(this.tilesA[tiles.row, column]);
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

    private IEnumerable<GameObject> Get_Matches_Vertically(GameObject tile)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(tile);
        var tiles = tile.GetComponent<s_Tiles>();

        if (tiles.row != 0)
        {
            for (int row = tiles.row - 1; row >= 0; row--)
            {
                if (tilesA[row, tiles.column] != null && tilesA[row, tiles.column].GetComponent<s_Tiles>().Is_Same_Type(tiles))
                {
                    matches.Add(tilesA[row, tiles.column]);
                }
                else
                    break;
            }
        }

        if (tiles.row != s_Constants.columns - 1)
        {
            for (int row = tiles.row + 1; row >= s_Constants.rows; row++)
            {
                if (tilesA[row, tiles.column] != null && tilesA[row, tiles.column].GetComponent<s_Tiles>().Is_Same_Type(tiles))
                {
                    matches.Add(tilesA[row, tiles.column]);
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
    public void Remove(GameObject tile)
    {
        tilesA[tile.GetComponent<s_Tiles>().row, tile.GetComponent<s_Tiles>().column] = null;
    }

    public s_MovedTileInfo Collapse(IEnumerable<int> columns)
    {
        s_MovedTileInfo collapseInfo = new s_MovedTileInfo();

        foreach(var column in columns)
        {
            for(int row = 0; row < s_Constants.rows - 1; row++)
            {
                if(tilesA[row, column] == null)
                {
                    for(int row2 = row + 1; row2 < s_Constants.rows; row2++)
                    {
                        if(tilesA[row2, column] != null)
                        {
                            tilesA[row, column] = tilesA[row2, column];
                            tilesA[row2, column] = null;

                            if(row2 - row > collapseInfo.maxMoveDistance)
                            {
                                collapseInfo.maxMoveDistance = row2 - row;
                            }

                            tilesA[row, column].GetComponent<s_Tiles>().row = row;
                            tilesA[row, column].GetComponent<s_Tiles>().column = column;

                            collapseInfo.Add_Tile(tilesA[row, column]);
                            break;
                        }
                    }
                }
            }
        }
        return collapseInfo;
    }

    public IEnumerable<s_TileInfo> Get_Empty_Items_On_Column(int column)
    {
        List<s_TileInfo> emptyItems = new List<s_TileInfo>();
        for(int row = 0; row < s_Constants.rows; row++)
        {
            if(tilesA[row, column] == null)
            {
                emptyItems.Add(new s_TileInfo() { Row = row, Column = column });
            }
        }
        return emptyItems;
    }
}