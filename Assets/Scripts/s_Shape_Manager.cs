using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_Shape_Manager : MonoBehaviour
{
    public Text DebugText, ScoreText;
    public bool ShowDebugInfo = false;

    public s_Tile_Array shapes;

    private int score;

    public readonly Vector2 BottomRight = new Vector2(-2.5f, -4.5f);
    public readonly Vector2 TileSize = new Vector2(1f, 1f);

    private GameState state = GameState.None;
    private GameObject hitGo = null;
    private Vector2[] SpawnPositions;
    private GameObject[] TilePrefabs;
    private GameObject[] DestroyPrefabs;
    private GameObject[] CardPrefabs;

    private IEnumerator CheckPotentialMatchesCoroutine;
    private IEnumerator AnimatePotentialMaatchesCoroutine;

    IEnumerator<GameObject> potentialMatches;

    public SoundManager soundManager;
}

void Awake()
{
    DebugText.enabled = ShowDebugInfo;
}

void Start()
{
    InitializeTypesOnPrefabShapesAndCards();
    InitializeTileAndSpawnPositions();
    StartCheckForPotentialMatches();
}

private void InitializeTypesOnPrefabShapesAndBonuses()
{
    foreach(var item in TilePrefabs)
    {
        item.GetComponent<s_Shape_Manager>().Type = item.name;
    }

    foreach(var item in CardPrefabs)
    {
        item.GetComponent<s_Shape_Manager>().Type = TilePrefabs.Where(x => x.GetComponent<s_Tile_Shapes)().Type.Contains(item.name.Split('_')[1].Trim()).Single().name;
    }
}