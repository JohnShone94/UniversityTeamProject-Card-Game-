using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_GameUtilities
{
    public static IEnumerator Game_Utilities(IEnumerable<GameObject> nextMatch)
    {
        for (float i = 1f; i >= 0.3; i -= 0.1f)
        {
            foreach (var item in nextMatch)
            {
                Color c = item.GetComponent<SpriteRenderer>().color;
                c.a = i;
                item.GetComponent<SpriteRenderer>().color = c;
            }
            yield return new WaitForSeconds(s_Constants.animOpacityFrameDelay);
        }
        for (float i = 0.3f; i >= 1; i += 0.1f)
        {
            foreach (var item in nextMatch)
            {
                Color c = item.GetComponent<SpriteRenderer>().color;
                c.a = i;
                item.GetComponent<SpriteRenderer>().color = c;
            }
            yield return new WaitForSeconds(s_Constants.animOpacityFrameDelay);
        }
    }

    public static bool Neighbour_Alignment(s_Tiles tileOne, s_Tiles tileTwo)
    {
        return (tileOne.column == tileTwo.column || tileOne.row == tileTwo.row) 
            && Mathf.Abs(tileOne.column - tileTwo.column) <= 1
            && Mathf.Abs(tileOne.row - tileTwo.row) <= 1;
    }

    public static IEnumerable<GameObject> Find_Next_Match(s_TileArray tileArray)
    {
        List<List<GameObject>> matches = new List<List<GameObject>>();

        for(int matchRow = 0; matchRow < s_Constants.rows; matchRow++)
        {
            for(int matchColumn = 0; matchColumn < s_Constants.columns; matchColumn ++)
            {
                var matchOne = checkHorizontalOne(matchRow, matchColumn, tileArray);
                var matchTwo = checkHorizontalTwo(matchRow, matchColumn, tileArray);
                var matchThree = checkHorizontalThree(matchRow, matchColumn, tileArray);
                var matchFour = checkVerticalOne(matchRow, matchColumn, tileArray);
                var matchFive = checkVerticalTwo(matchRow, matchColumn, tileArray);
                var matchSix = checkVerticalThree(matchRow, matchColumn, tileArray);

                if (matchOne != null) matches.Add(matchOne);
                if (matchTwo != null) matches.Add(matchTwo);
                if (matchThree != null) matches.Add(matchThree);
                if (matchFour != null) matches.Add(matchFour);
                if (matchFive != null) matches.Add(matchFive);
                if (matchSix != null) matches.Add(matchSix);

                if (matches.Count >= 3)
                {
                    return matches[UnityEngine.Random.Range(0, matches.Count - 1)];
                }

                if(matchRow >= s_Constants.rows / 2 && matches.Count > 0 && matches.Count <= 2)
                {
                    return matches[UnityEngine.Random.Range(0, matches.Count - 1)];
                }
            }
        }
        return null;
    }

    public static List<GameObject> checkHorizontalOne(int row, int column, s_TileArray tiles)
    {
        if (column <= s_Constants.columns - 2)
        {
            if (tiles[row, column].GetComponent<s_Tiles>().
                Is_Same_Type(tiles[row, column + 1].GetComponent<s_Tiles>()))
            {
                if (row >= 1 && column >= 1)
                    if (tiles[row, column].GetComponent<s_Tiles>().
                    Is_Same_Type(tiles[row - 1, column - 1].GetComponent<s_Tiles>()))
                        return new List<GameObject>()
                                {
                                    tiles[row, column],
                                    tiles[row, column + 1],
                                    tiles[row - 1, column - 1]
                                };

                if (row <= s_Constants.rows - 2 && column >= 1)
                    if (tiles[row, column].GetComponent<s_Tiles>().
                    Is_Same_Type(tiles[row + 1, column - 1].GetComponent<s_Tiles>()))
                        return new List<GameObject>()
                                {
                                    tiles[row, column],
                                    tiles[row, column + 1],
                                    tiles[row + 1, column - 1]
                                };
            }
        }
        return null;
    }


    public static List<GameObject> checkHorizontalTwo(int row, int column, s_TileArray tiles)
    {
        if (column <= s_Constants.columns - 3)
        {
            if (tiles[row, column].GetComponent<s_Tiles>().
                Is_Same_Type(tiles[row, column + 1].GetComponent<s_Tiles>()))
            {

                if (row >= 1 && column <= s_Constants.columns - 3)
                    if (tiles[row, column].GetComponent<s_Tiles>().
                    Is_Same_Type(tiles[row - 1, column + 2].GetComponent<s_Tiles>()))
                        return new List<GameObject>()
                                {
                                    tiles[row, column],
                                    tiles[row, column + 1],
                                    tiles[row - 1, column + 2]
                                };

                if (row <= s_Constants.rows - 2 && column <= s_Constants.columns - 3)
                    if (tiles[row, column].GetComponent<s_Tiles>().
                    Is_Same_Type(tiles[row + 1, column + 2].GetComponent<s_Tiles>()))
                        return new List<GameObject>()
                                {
                                    tiles[row, column],
                                    tiles[row, column + 1],
                                    tiles[row + 1, column + 2]
                                };
            }
        }
        return null;
    }

    public static List<GameObject> checkHorizontalThree(int row, int column, s_TileArray tiles)
    {
        if (column <= s_Constants.columns - 4)
        {
            if (tiles[row, column].GetComponent<s_Tiles>().
               Is_Same_Type(tiles[row, column + 1].GetComponent<s_Tiles>()) &&
               tiles[row, column].GetComponent<s_Tiles>().
               Is_Same_Type(tiles[row, column + 3].GetComponent<s_Tiles>()))
            {
                return new List<GameObject>()
                                {
                                    tiles[row, column],
                                    tiles[row, column + 1],
                                    tiles[row, column + 3]
                                };
            }
        }
        if (column >= 2 && column <= s_Constants.columns - 2)
        {
            if (tiles[row, column].GetComponent<s_Tiles>().
               Is_Same_Type(tiles[row, column + 1].GetComponent<s_Tiles>()) &&
               tiles[row, column].GetComponent<s_Tiles>().
               Is_Same_Type(tiles[row, column - 2].GetComponent<s_Tiles>()))
            {
                return new List<GameObject>()
                                {
                                    tiles[row, column],
                                    tiles[row, column + 1],
                                    tiles[row, column -2]
                                };
            }
        }
        return null;
    }

    public static List<GameObject> checkVerticalOne(int row, int column, s_TileArray tiles)
    {
        if (row <= s_Constants.rows - 2)
        {
            if (tiles[row, column].GetComponent<s_Tiles>().
                Is_Same_Type(tiles[row + 1, column].GetComponent<s_Tiles>()))
            {
                if (column >= 1 && row >= 1)
                    if (tiles[row, column].GetComponent<s_Tiles>().
                    Is_Same_Type(tiles[row - 1, column - 1].GetComponent<s_Tiles>()))
                        return new List<GameObject>()
                                {
                                    tiles[row, column],
                                    tiles[row + 1, column],
                                    tiles[row - 1, column -1]
                                };

                if (column <= s_Constants.columns - 2 && row >= 1)
                    if (tiles[row, column].GetComponent<s_Tiles>().
                    Is_Same_Type(tiles[row - 1, column + 1].GetComponent<s_Tiles>()))
                        return new List<GameObject>()
                                {
                                    tiles[row, column],
                                    tiles[row + 1, column],
                                    tiles[row - 1, column + 1]
                                };
            }
        }
        return null;
    }

    public static List<GameObject> checkVerticalTwo(int row, int column, s_TileArray tiles)
    {
        if (row <= s_Constants.rows - 3)
        {
            if (tiles[row, column].GetComponent<s_Tiles>().
                Is_Same_Type(tiles[row + 1, column].GetComponent<s_Tiles>()))
            {
                if (column >= 1)
                    if (tiles[row, column].GetComponent<s_Tiles>().
                    Is_Same_Type(tiles[row + 2, column - 1].GetComponent<s_Tiles>()))
                        return new List<GameObject>()
                                {
                                    tiles[row, column],
                                    tiles[row + 1, column],
                                    tiles[row + 2, column -1]
                                };

                if (column <= s_Constants.columns - 2)
                    if (tiles[row, column].GetComponent<s_Tiles>().
                    Is_Same_Type(tiles[row + 2, column + 1].GetComponent<s_Tiles>()))
                        return new List<GameObject>()
                                {
                                    tiles[row, column],
                                    tiles[row+1, column],
                                    tiles[row + 2, column + 1]
                                };
            }
        }
        return null;
    }

    public static List<GameObject> checkVerticalThree(int row, int column, s_TileArray tiles)
    {
        if (row <= s_Constants.rows - 4)
        {
            if (tiles[row, column].GetComponent<s_Tiles>().
               Is_Same_Type(tiles[row + 1, column].GetComponent<s_Tiles>()) &&
               tiles[row, column].GetComponent<s_Tiles>().
               Is_Same_Type(tiles[row + 3, column].GetComponent<s_Tiles>()))
            {
                return new List<GameObject>()
                                {
                                    tiles[row, column],
                                    tiles[row + 1, column],
                                    tiles[row + 3, column]
                                };
            }
        }
        if (row >= 2 && row <= s_Constants.rows - 2)
        {
            if (tiles[row, column].GetComponent<s_Tiles>().
               Is_Same_Type(tiles[row + 1, column].GetComponent<s_Tiles>()) &&
               tiles[row, column].GetComponent<s_Tiles>().
               Is_Same_Type(tiles[row - 2, column].GetComponent<s_Tiles>()))
            {
                return new List<GameObject>()
                                {
                                    tiles[row, column],
                                    tiles[row + 1, column],
                                    tiles[row - 2, column]
                                };
            }
        }
        return null;
    }

}
