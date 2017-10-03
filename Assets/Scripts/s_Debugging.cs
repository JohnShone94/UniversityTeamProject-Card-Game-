using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_Debugging : MonoBehaviour
{
    public static string[,] FillTilesArray()
    {
        string[,] shapes = new string[s_Constants.rows, s_Constants.columns];
        TextAsset txt = Resources.Load("Level") as TextAsset;
        string level = txt.text;

        string[] lines = level.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        for(int row = s_Constants.rows - 1; row >= 0; row--)
        {
            string[] items = lines[row].Split("|");
            for(int column = 0; column < s_Constants.columns; column++)
            {
                shapes[row, column] = items[column];
            }
        }
        return shapes;
    }

    private GameObject GetSpecificTileOrCard(string info)
    {
        var tokens = info.Split('_');

        if (tokens.Count() == 1)
        {
            foreach (var item in TilePrefabs)
            {
                if (item.GetComponent<s_Tile_Shapes>().Type.Contains(tokens[0].Trim()))
                    return item;
            }

        }
        else if (tokens.Count() == 2 && tokens[1].Trim() == "B")
        {
            foreach (var item in CardPrefabs)
            {
                if (item.name.Contains(tokens[0].Trim()))
                    return item;
            }
        }

        throw new System.Exception("Wrong type, check your premade level");
    }

    public void InitialiseTileAndSpawnPos()
    {
        InitializeVariables();
        var premadeLevel = DebugUtilities.FillShapesArrayFromResourcesData();
        if(tiles != null)
        {
            DestroyAllTiles();
        }

        shapes = new s_Tile_Array();
        SpawnPosotions = new Vector2[s_Constants.columns];

        for(int row = 0; row < s_Constants.rows; row++)
        {
            for(int column = 0; column < s_Constants.columns; column++)
            {
                GameObject newTile = null;
                newTile = GetSpecificTileOrCard(premadeLevel[row, column]);
                InstantiateAndPlaceNewTile(row, column, newTile);

            }
        }
        SetupSpawnPositions();
    }
}
