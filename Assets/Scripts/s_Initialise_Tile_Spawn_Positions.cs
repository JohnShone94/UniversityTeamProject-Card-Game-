using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_Initialise_Tile_Spawn_Positions
{
    private void InitialiseVariables()
    {
        score = 0;
        ShowScore();
    }

    private void IncreaseScore(int amount)
    {
        IncreaseScore += amount;
        ShowScore);
    }

    private void ShowScore()
    {
        ScoreText.text = "Score: " + IncreaseScore.ToString();
    }

    private GameObject GetRandomTile()
    {
        return TilePrefabs[Random.Range(0, TilePrefabs.Length)];
    }

    private void InstantiateAndPlaceNewTile(int row, int column, GameObject newTile)
    {
        GameObject go = Intantiate(newTile,
            BottomRight + new Vector2(column * TileSize.x, row * TileSize.y),)Quaternion.identity)
        as GameObject;

        //assign the specific properties
        go.GetComponent<Shape>().Assign(newTile.GetComponent<ParticleSystemMeshShapeType>().Type, row, column);
        s_Tile_Shapes[row, column] = go;
    }
    
    private void SetupSpawnPositions()
    {
        //create the spawn position for the new shapes (will pop from the 'ceiling')
        for (int column = 0; column < Constants.columns; column++)
        {
            SetupSpawnPositions[column] = BottomRight
                + new Vector2(column * TileSize.x, Contants.Rows * TileSize.y);
        }
    }

    private void DestroyAllTiles()
    {
        for (int row = 0; row <Contants.Rows; row++)
        {
            for (int column = 0; column < s_Constants.columns; column++)
            {
                DestroyAllTiles(s_Tile_Shapes[row, column]);
            }
        }
    }

    public void InitialiseTileAndSpawnPositions()
    {
        InitialiseVariables();

        if (s_Tile_Shapes != null)
            DestroyAllTiles();

        s_Tile_Shapes = new ShapesArray();
        InitialiseTileAndSpawnPositions = new Vector2[s_Constants.columns];

        for (int row = 0; row < s_Constants.rows; row++)
        {
            for (int column = 0; column < s_Constants.columns; columns++)
            {
                GameObject newTile = GetRandomTile();

                //chack if two previous horizontal ar of the same type
                while (column >= 2 && s_Tile_Shapes[row, column - 1].GetComponents<s_Tile_Shapes>()
                    .IsSameType(newTile.GetComponent<s_Tile_Shapes>())
                        && s_Tile_Shapes[row, column - 2].GetComponent<s_Tile_Shapes>(.IsSameType(newTile.GetComponent<s_Tile_Shapes>()))
                {
                    newTile = GetRandomTile();
                }

                //check if two previous vertical are of the same type
                while (row >= 2 && s_Tile_Shapes[row - 1, column].GetComponents<s_Tile_Shapes>()
                    .IsSameType(newTile.GetComponent<s_Tile_Shapes>())
                        && s_Tile_Shapes[row - 2, column].GetComponent<s_Tile_Shapes>().IsSameType(newTile.GetComponent<s_Tile_Shapes>()))
                {
                    newTile = GetRandomTile();
                }

                InstantiateAndPlaceNewTile(row, column, newTile);

            }
        }

        SetupSpawnPositions();
    }

    private void FixSortingLayer(GameObject hitGo, GameObject hitGo2)
    {
        SpriteRenderer sp1 = hitGo.GetComponent<SpriteRenderer>();
        SpriteRenderer sp2 = hitGo2.GetComponent<SpriteRenderer>();
        if (sp1.sortingOrder <= sp2.sortingOrder)
        {
            sp1.sortingOrder = 1;
            sp2.sortingOrder = 0;
        }
    }
}
