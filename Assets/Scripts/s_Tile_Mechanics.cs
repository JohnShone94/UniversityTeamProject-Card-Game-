using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class s_Tile_Mechanics : MonoBehaviour
{
	private GameObject GetRandomExplosion()
	{
		return ExplosionPrefabs [Random.Range (0, ExplosionPrefabs.Length)];
	}

	private GameObject GetBonusType(string type)
	{
		string color = type.Split ('_') [1].Trim ();
		foreach (var item in BonusPrefabs) 
		{
			if (item.GetComponent<s_Tile_Shapes> ().Type.Contains (color))
				return item;
		}
		throw new System.Exception("Wrong type");
	}

	private void RemoveFromScene(GameObject item)
	{
		GameObject explosion = GetRandomExplosion();
		var newExplosion = Instantiate(explosion, item.transform.position, Quaternion.identity) as GameObject;
		Destroy(newExplosion, Constants.ExplosionDuration);
		Destroy(item);
	}

	private void MoveAndAnimate(IEnumerable<GameObject> movedGameObjects, int distance)
	{
		foreach (var item in movedGameObjects)
		{
			item.transform.positionTo(Constants.MoveAnimationMinDuration * distance, BottomRight +
				new Vector2(item.GetComponent<s_Tile_Shapes>().Column * CandySize.x, item.GetComponent<s_Tile_Shapes>().Row * CandySize.y));
		}
	}

	private AlteredCandyInfo CreateNewCandyInSpecificColumns(IEnumerable<int> columnsWithMissingCandy)
	{
		AlteredCandyInfo newCandyInfo = new AlteredCandyInfo();

		//find how many null values the column has
		foreach (int column in columnsWithMissingCandy)
		{
			var emptyItems = shapes.GetEmptyItemsOnColumn(column);
			foreach (var item in emptyItems)
			{
				var go = GetRandomCandy();
				GameObject newCandy = Instantiate(go, SpawnPositions[column], Quaternion.identity)
					as GameObject;

				newCandy.GetComponent<s_Tile_Shapes>().Assign(go.GetComponent<s_Tile_Shapes>().Type, item.Row, item.Column);

				if (Constants.Rows - item.Row > newCandyInfo.MaxDistance)
					newCandyInfo.MaxDistance = Constants.Rows - item.Row;

				shapes[item.Row, item.Column] = newCandy;
				newCandyInfo.AddCandy(newCandy);
			}
		}
		return newCandyInfo;
	}

	private void CreateBonus(Shape hitGoCache)
	{
		GameObject Bonus = Instantiate(GetBonusFromType(hitGoCache.Type), BottomRight
			+ new Vector2(hitGoCache.Column * CandySize.x,
				hitGoCache.Row * CandySize.y), Quaternion.identity)
			as GameObject;
		shapes[hitGoCache.Row, hitGoCache.Column] = Bonus;
		var BonusShape = Bonus.GetComponent<Shape>();
		//will have the same type as the "normal" candy
		BonusShape.Assign(hitGoCache.Type, hitGoCache.Row, hitGoCache.Column);
		//add the proper Bonus type
		BonusShape.Bonus |= BonusType.DestroyWholeRowColumn;
	}

	void Update()
	{
		if (ShowDebugInfo)
			DebugText.text = DebugUtilities.GetArrayContents(shapes);

		if (state == GameState.None)
		{
			//user has clicked or touched
			if (Input.GetMouseButtonDown(0))
			{
				//get the hit position
				var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (hit.collider != null) //we have a hit!!!
				{
					hitGo = hit.collider.gameObject;
					state = GameState.SelectionStarted;
				}

			}
		}

		else if (state == GameState.SelectionStarted)
		{
			//user dragged
			if (Input.GetMouseButton(0))
			{


				var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				//we have a hit
				if (hit.collider != null && hitGo != hit.collider.gameObject)
				{

					//user did a hit, no need to show him hints 
					StopCheckForPotentialMatches();

					//if the two shapes are diagonally aligned (different row and column), just return
					if (!Utilities.AreVerticalOrHorizontalNeighbors(hitGo.GetComponent<Shape>(),
						hit.collider.gameObject.GetComponent<Shape>()))
					{
						state = GameState.None;
					}
					else
					{
						state = GameState.Animating;
						FixSortingLayer(hitGo, hit.collider.gameObject);
						StartCoroutine(FindMatchesAndCollapse(hit));
					}
				}
			}
		}
	}

	private IEnumerator FindMatchesAndCollapse(RaycastHit2D hit2)
	{
		//get the second item that was part of the swipe
		var hitGo2 = hit2.collider.gameObject;
		shapes.Swap(hitGo, hitGo2);

		//move the swapped ones
		hitGo.transform.positionTo(Constants.AnimationDuration, hitGo2.transform.position);
		hitGo2.transform.positionTo(Constants.AnimationDuration, hitGo.transform.position);
		yield return new WaitForSeconds(Constants.AnimationDuration);

		//get the matches via the helper methods
		var hitGomatchesInfo = shapes.GetMatches(hitGo);
		var hitGo2matchesInfo = shapes.GetMatches(hitGo2);

		var totalMatches = hitGomatchesInfo.MatchedCandy
			.Union(hitGo2matchesInfo.MatchedCandy).Distinct();

		//if user's swap didn't create at least a 3-match, undo their swap
		if (totalMatches.Count() < Constants.MinimumMatches)
		{
			hitGo.transform.positionTo(Constants.AnimationDuration, hitGo2.transform.position);
			hitGo2.transform.positionTo(Constants.AnimationDuration, hitGo.transform.position);
			yield return new WaitForSeconds(Constants.AnimationDuration);

			shapes.UndoSwap();
		}

		//if more than 3 matches and no Bonus is contained in the line, we will award a new Bonus
		bool addBonus = totalMatches.Count() >= Constants.MinimumMatchesForBonus &&
			!BonusTypeUtilities.ContainsDestroyWholeRowColumn(hitGomatchesInfo.BonusesContained) &&
			!BonusTypeUtilities.ContainsDestroyWholeRowColumn(hitGo2matchesInfo.BonusesContained);
		
		Shape hitGoCache = null;
		if (addBonus)
		{
			//get the game object that was of the same type
			var sameTypeGo = hitGomatchesInfo.MatchedCandy.Count() > 0 ? hitGo : hitGo2;
			hitGoCache = sameTypeGo.GetComponent<Shape>();
		}

		int timesRun = 1;
		while (totalMatches.Count() >= Constants.MinimumMatches)
		{
			//increase score
			IncreaseScore((totalMatches.Count() - 2) * Constants.Match3Score);

			if (timesRun >= 2)
				IncreaseScore(Constants.SubsequentMatchScore);

			soundManager.PlayCrincle();

			foreach (var item in totalMatches)
			{
				shapes.Remove(item);
				RemoveFromScene(item);
			}

			//check and instantiate Bonus if needed
			if (addBonus)
				CreateBonus(hitGoCache);

			addBonus = false;

			//get the columns that we had a collapse
			var columns = totalMatches.Select(go => go.GetComponent<Shape>().Column).Distinct();

			//the order the 2 methods below get called is important!!!
			//collapse the ones gone
			var collapsedCandyInfo = shapes.Collapse(columns);
			//create new ones
			var newCandyInfo = CreateNewCandyInSpecificColumns(columns);

			int maxDistance = Mathf.Max(collapsedCandyInfo.MaxDistance, newCandyInfo.MaxDistance);

			MoveAndAnimate(newCandyInfo.AlteredCandy, maxDistance);
			MoveAndAnimate(collapsedCandyInfo.AlteredCandy, maxDistance);



			//will wait for both of the above animations
			yield return new WaitForSeconds(Constants.MoveAnimationMinDuration * maxDistance);

			//search if there are matches with the new/collapsed items
			totalMatches = shapes.GetMatches(collapsedCandyInfo.AlteredCandy).
				Union(shapes.GetMatches(newCandyInfo.AlteredCandy)).Distinct();



			timesRun++;
		}

		state = GameState.None;
		StartCheckForPotentialMatches();
	}
}
