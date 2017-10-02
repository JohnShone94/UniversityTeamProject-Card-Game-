using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_Show_Next_Match
{
    public static IEnumerator showNextMatch(IEnumerable<GameObject> nextMatch)
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

    public static bool neighbourAlignment(s_Tile_Shapes t1, s_Tile_Shapes t2)
    {
        return (t1.column == t2.column || t1.row == t2.row) 
            && Mathf.Abs(t1.column - t2.column) <= 1
            && Mathf.Abs(t1.row - t2.row) <= 1;
    }

    public static IEnumerable<GameObject> findNextMatch(s_Tile_Shapes_Array ta)
    {
        List<List<GameObject>> matches = new List<List<GameObject>>();

        for(int mRow = 0; mRow < s_Constants.rows; mRow++)
        {
            for(int mColumn = 0; mColumn < s_Constants.columns; mColumn ++)
            {
                var matchOne = checkHorizontalOne(mRow, mColumn, ta);
                var matchTwo = checkHorizontalTwo(mRow, mColumn, ta);
                var matchThree = checkHorizontalThree(mRow, mColumn, ta);
                var matchFour = checkVerticalOne(mRow, mColumn, ta);
                var matchFive = checkVerticalTwo(mRow, mColumn, ta);
                var matchSix = checkVerticalThree(mRow, mColumn, ta);

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

                if(mRow >= s_Constants.rows / 2 &&matches.Count > 0 && matches.Count <= 2)
                {
                    return matches[UnityEngine.Random.Range(0, matches.Count - 1)];
                }
            }
        }
        return null;
    }
}
