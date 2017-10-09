using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class s_TileManager : MonoBehaviour
{
    public Text DebugText, ScoreText, TimeRemainingText;
    public bool ShowDebugInfo = false;

    public s_TileArray tiles;

    private int score;

    public readonly Vector2 BottomRight = new Vector2(-1f, -1f);
    public readonly Vector2 TileSize = new Vector2(2.56f, 2.56f);

    private GameState state = GameState.None;
    private GameObject hitTile = null;
    private Vector2[] SpawnPositions;
    public GameObject[] TilePrefabs;

    private IEnumerator CheckPotentialMatchesCoroutine;
    private IEnumerator AnimatePotentialMatchesCoroutine;

    IEnumerable<GameObject> potentialMatches;

	int timeRemaining = 180;
	bool gameInPlay = true;

    void Awake()
    {
        DebugText.enabled = ShowDebugInfo;
    }

    void Start()
    {
		StartCoroutine (OneSecond ());
        Initialise_Types_On_Prefab_Shapes_And_Cards();
        Initialise_Tile_And_Spawn_Positions();
        Start_Check_For_Potential_Matches();
    }

	IEnumerator OneSecond(){
		while (1 == 1 && gameInPlay) {
			yield return new WaitForSeconds (1.0f);
			timeRemaining--;
			TimeRemainingText.text = "Time: " + timeRemaining;
			if (timeRemaining <= 0) {
				EndGame ();
				break;
			}
		}
	}

	void EndGame() {
		gameInPlay = false;
		GetComponent<s_Scoreboard> ().checkForHighScore (score);
	}

    private void Initialise_Types_On_Prefab_Shapes_And_Cards()
    {
        foreach (var tile in TilePrefabs)
        {
            tile.GetComponent<s_Tiles>().Type = tile.name;
        }

      /*  foreach (var card in CardPrefabs)
        {
            card.GetComponent<s_Tiles>().Type = TilePrefabs.Where(x => x.GetComponent<s_Tiles>().Type.Contains(card.name.Split('_')[1].Trim())).Single().name;
        }*/
    }


    public void Initialize_Candy_And_Spawn_Positions_From_Premade_Level()
    {
        Initialise_Variables();

        var premadeLevel = s_Debugging.Fill_Tiles_Array();
        if (tiles != null)
        {
            Destroy_All_Tiles();
        }

        tiles = new s_TileArray();
        SpawnPositions = new Vector2[s_Constants.columns];

        for (int row = 0; row < s_Constants.rows; row++)
        {
            for (int column = 0; column < s_Constants.columns; column++)
            {

                GameObject newTile = null;

                newTile = Get_Specific_Tile_Or_Card(premadeLevel[row, column]);

                Instantiate_And_Place_New_Tile(row, column, newTile);

            }
        }

        Setup_Spawn_Positions();
    }

    public void Initialise_Tile_And_Spawn_Positions()
    {
        Initialise_Variables();

        if (tiles != null)
            Destroy_All_Tiles();

        tiles = new s_TileArray();
        SpawnPositions = new Vector2[s_Constants.columns];

        for (int row = 0; row < s_Constants.rows; row++)
        {
            for (int columns = 0; columns < s_Constants.columns; columns++)
            {
                GameObject newTile = Get_Random_Tile();

                while (columns >= 2 && tiles[row, columns - 1].GetComponent<s_Tiles>().Is_Same_Type(newTile.GetComponent<s_Tiles>())
                        && tiles[row, columns - 2].GetComponent<s_Tiles>().Is_Same_Type(newTile.GetComponent<s_Tiles>()))
                {
                    newTile = Get_Random_Tile();
                }

                while (row >= 2 && tiles[row - 1, columns].GetComponent<s_Tiles>().Is_Same_Type(newTile.GetComponent<s_Tiles>())
                        && tiles[row - 2, columns].GetComponent<s_Tiles>().Is_Same_Type(newTile.GetComponent<s_Tiles>()))
                {
                    newTile = Get_Random_Tile();
                }

                Instantiate_And_Place_New_Tile(row, columns, newTile);

            }
        }

        Setup_Spawn_Positions();
    }

    private void Instantiate_And_Place_New_Tile(int row, int column, GameObject newTile)
    {
        GameObject go = Instantiate(newTile, BottomRight + new Vector2(column * TileSize.x, row * TileSize.y), Quaternion.identity) as GameObject;

        go.GetComponent<s_Tiles>().Assign_Card(newTile.GetComponent<s_Tiles>().Type, row, column);
        tiles[row, column] = go;
    }

    private void Setup_Spawn_Positions()
    {
        for (int column = 0; column < s_Constants.columns; column++)
        {
            SpawnPositions[column] = BottomRight
                + new Vector2(column * TileSize.x, s_Constants.rows * TileSize.y);
        }
    }

    private void Destroy_All_Tiles()
    {
        for (int row = 0; row < s_Constants.rows; row++)
        {
            for (int column = 0; column < s_Constants.columns; column++)
            {
                Destroy(tiles[row, column]);
            }
        }
    }

    void Update()
    {
		if (!gameInPlay) {
			return;
		}

        if (ShowDebugInfo)
        {
            DebugText.text = s_Debugging.Get_Array_Contents(tiles);
        }

        if (state == GameState.None)
        {

            if (Input.GetMouseButtonDown(0))
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider != null)
                {
                    hitTile = hit.collider.gameObject;
                    state = GameState.SelectionStarted;
                    Debug.Log("Selected");
                }

            }
        }

        if (state == GameState.SelectionStarted)
        {
            if (Input.GetMouseButton(0))
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);


                if (hit.collider != null && hitTile != hit.collider.gameObject)
                {
                    Stop_Check_For_Potential_Matches();
                    if (!s_GameUtilities.Neighbour_Alignment(hitTile.GetComponent<s_Tiles>(), hit.collider.gameObject.GetComponent<s_Tiles>()))
                    {
                        state = GameState.None;
                    }
                    else
                    {
                        state = GameState.Animating;
                        Fix_Sorting_Layer(hitTile, hit.collider.gameObject);
                        StartCoroutine(Find_Matches_And_Collapse(hit));
                    }
                }
            }
        }
    }

    private void Fix_Sorting_Layer(GameObject hitTile, GameObject hitTileTwo)
    {
        SpriteRenderer sp1 = hitTile.GetComponent<SpriteRenderer>();
        SpriteRenderer sp2 = hitTileTwo.GetComponent<SpriteRenderer>();
        if (sp1.sortingOrder <= sp2.sortingOrder)
        {
            sp1.sortingOrder = 1;
            sp2.sortingOrder = 0;
        }
    }

    private IEnumerator Find_Matches_And_Collapse(RaycastHit2D hit2)
    {

        var hitTileTwo = hit2.collider.gameObject;
        tiles.Swap(hitTile, hitTileTwo);

        hitTile.transform.positionTo(s_Constants.animationTime, hitTileTwo.transform.position);
        hitTileTwo.transform.positionTo(s_Constants.animationTime, hitTile.transform.position);
        yield return new WaitForSeconds(s_Constants.animationTime);

        var hitGomatchesInfo = tiles.Get_Matches(hitTile);
        var hitGo2matchesInfo = tiles.Get_Matches(hitTileTwo);

        var totalMatches = hitGomatchesInfo.Matched_Tiles.Union(hitGo2matchesInfo.Matched_Tiles).Distinct();

        if (totalMatches.Count() < s_Constants.minTilesToMatch)
        {
            hitTile.transform.positionTo(s_Constants.animationTime, hitTileTwo.transform.position);
            hitTileTwo.transform.positionTo(s_Constants.animationTime, hitTile.transform.position);
            yield return new WaitForSeconds(s_Constants.animationTime);

            tiles.Undo_Swap();
        }

       // bool addBonus = totalMatches.Count() >= s_Constants.minTilesToBonusMatch &&
            //!s_Card_Utilities.Includes_Remove_Whole_Row_Column(hitGomatchesInfo.Card_Contained) &&
            //!s_Card_Utilities.Includes_Remove_Whole_Row_Column(hitGo2matchesInfo.Card_Contained);

        s_Tiles hitGoCache = null;

        /*if (addBonus)
        {
            var sameTypeGo = hitGomatchesInfo.Matched_Tiles.Count() > 0 ? hitTile : hitTileTwo;
            hitGoCache = sameTypeGo.GetComponent<s_Tiles>();
        }*/

        int timesRun = 1;

        while (totalMatches.Count() >= s_Constants.minTilesToMatch)
        {
            Increase_Score((totalMatches.Count() - 2) * s_Constants.scoreBasicMatch);

            if (timesRun >= 2)
            {
                Increase_Score(s_Constants.scoreSubsequentMatch);
            }

            foreach (var item in totalMatches)
            {
                tiles.Remove(item);
                Remove_From_Scene(item);
            }

            /* if (addBonus)
             {
                 Create_Bonus(hitGoCache);
             }*/

           // addBonus = false;

            var columns = totalMatches.Select(go => go.GetComponent<s_Tiles>().column).Distinct();

            var collapsedTileInfo = tiles.Collapse(columns);

            var newTileInfo = Create_New_Tile_In_Specific_Columns(columns);

            int maxDistance = Mathf.Max(collapsedTileInfo.maxMoveDistance, newTileInfo.maxMoveDistance);

            Move_And_Animate(newTileInfo.Moved_Tile, maxDistance);
            Move_And_Animate(collapsedTileInfo.Moved_Tile, maxDistance);

            yield return new WaitForSeconds(s_Constants.minMovingAnimTime * maxDistance);

            totalMatches = tiles.Get_Matches(collapsedTileInfo.Moved_Tile).Union(tiles.Get_Matches(newTileInfo.Moved_Tile)).Distinct();

            timesRun++;
        }

        state = GameState.None;
        Start_Check_For_Potential_Matches();
    }

    /*private void Create_Bonus(s_Tiles hitTileCache)
    {
        GameObject Bonus = Instantiate(GetBonusType(hitTileCache.Type), BottomRight
            + new Vector2(hitTileCache.column * TileSize.x,
                hitTileCache.row * TileSize.y), Quaternion.identity) as GameObject;

        tiles[hitTileCache.row, hitTileCache.column] = Bonus;
        var BonusShape = Bonus.GetComponent<s_Tiles>();

        BonusShape.Assign_Card(hitTileCache.Type, hitTileCache.row, hitTileCache.column);

        BonusShape.Card |= s_CardType.RemoveWholeRowColumn;
    }*/

    private s_MovedTileInfo Create_New_Tile_In_Specific_Columns(IEnumerable<int> columnsWithMissingTiles)
    {
        s_MovedTileInfo newTileInfo = new s_MovedTileInfo();


        foreach (int column in columnsWithMissingTiles)
        {
            var emptyItems = tiles.Get_Empty_Items_On_Column(column);
            foreach (var item in emptyItems)
            {
                var go = Get_Random_Tile();
                GameObject newTile = Instantiate(go, SpawnPositions[column], Quaternion.identity)
                    as GameObject;

                newTile.GetComponent<s_Tiles>().Assign_Card(go.GetComponent<s_Tiles>().Type, item.Row, item.Column);

                if (s_Constants.rows - item.Row > newTileInfo.maxMoveDistance)
                    newTileInfo.maxMoveDistance = s_Constants.rows - item.Row;

                tiles[item.Row, item.Column] = newTile;
                newTileInfo.Add_Tile(newTile);
            }
        }
        return newTileInfo;
    }

    private GameObject Get_Random_Tile()
    {
        return TilePrefabs[Random.Range(0, TilePrefabs.Length)];
    }

    private void Move_And_Animate(IEnumerable<GameObject> movedGameObjects, int distance)
    {
        foreach (var item in movedGameObjects)
        {
            item.transform.positionTo(s_Constants.minMovingAnimTime * distance, BottomRight +
                 new Vector2(item.GetComponent<s_Tiles>().column * TileSize.x, item.GetComponent<s_Tiles>().row * TileSize.y));
        }
    }

    private void Remove_From_Scene(GameObject item)
    {
        // GameObject explosion = Get_Random_Explosion();
        //  var newExplosion = Instantiate(explosion, item.transform.position, Quaternion.identity) as GameObject;
        //Destroy(newExplosion, s_Constants.removeAnimTime);
        Destroy(item);
    }

    private void Initialise_Variables()
    {
        score = 0;
        Show_Score();
    }

    private void Increase_Score(int amount)
    {
        score += amount;
        Show_Score();
    }

    private void Show_Score()
    {
        ScoreText.text = "Score: " + score.ToString();
    }

    /*private GameObject Get_Random_Explosion()
    {
        return DestroyPrefabs[Random.Range(0, DestroyPrefabs.Length)];
    }*/

    /* private GameObject GetBonusType(string type)
    {
        string color = type.Split('_')[1].Trim();
        foreach (var item in CardPrefabs)
        {
            if (item.GetComponent<s_Tiles>().Type.Contains(color))
            {
                return item;
            }
        }
        throw new System.Exception("Wrong type");
    }*/

    private void Start_Check_For_Potential_Matches()
    {
        Stop_Check_For_Potential_Matches();
        CheckPotentialMatchesCoroutine = Check_Potential_Matches();
        StartCoroutine(CheckPotentialMatchesCoroutine);
    }

    private void Stop_Check_For_Potential_Matches()
    {
        if (AnimatePotentialMatchesCoroutine != null)
        {
            StopCoroutine(AnimatePotentialMatchesCoroutine);
        }
        if (CheckPotentialMatchesCoroutine != null)
        {
            StopCoroutine(CheckPotentialMatchesCoroutine);
        }
        Reset_Opacity_On_Potential_Matches();
    }

    private void Reset_Opacity_On_Potential_Matches()
    {
        if (potentialMatches != null)
            foreach (var item in potentialMatches)
            {
                if (item == null) break;

                Color c = item.GetComponent<SpriteRenderer>().color;
                c.a = 1.0f;
                item.GetComponent<SpriteRenderer>().color = c;
            }
    }

    private IEnumerator Check_Potential_Matches()
    {
        yield return new WaitForSeconds(s_Constants.timeBeforeMatchCheck);
        potentialMatches = s_GameUtilities.Find_Next_Match(tiles);
        if (potentialMatches != null)
        {
            while (true)
            {
                AnimatePotentialMatchesCoroutine = s_GameUtilities.Game_Utilities(potentialMatches);
                StartCoroutine(AnimatePotentialMatchesCoroutine);
                yield return new WaitForSeconds(s_Constants.timeBeforeMatchCheck);
            }
        }

        if(potentialMatches == null)
        {
            Destroy_All_Tiles();

            tiles = new s_TileArray();
            SpawnPositions = new Vector2[s_Constants.columns];
            var premadeLevel = s_Debugging.Fill_Tiles_Array();

            for (int row = 0; row < s_Constants.rows; row++)
            {
                for (int column = 0; column < s_Constants.columns; column++)
                {

                    GameObject newTile = null;

                    newTile = Get_Random_Tile();

                    Instantiate_And_Place_New_Tile(row, column, newTile);

                }
            }

            Setup_Spawn_Positions();


        }
    }

    private GameObject Get_Specific_Tile_Or_Card(string info)
    {
        var tokens = info.Split('_');

        if (tokens.Count() == 1)
        {
            foreach (var item in TilePrefabs)
            {
                if (item.GetComponent<s_Tiles>().Type.Contains(tokens[0].Trim()))
                    return item;
            }

        }
        /*else if (tokens.Count() == 2 && tokens[1].Trim() == "B")
        {
            foreach (var item in CardPrefabs)
            {
                if (item.name.Contains(tokens[0].Trim()))
                    return item;
            }
        }*/

        throw new System.Exception("Wrong type, check your premade level");
    }
}
