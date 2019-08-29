using System;
using System.Collections;
using System.Linq;
using ResourcesObjects;
using UnityEngine;
using UnityEngine.UI;

public class GridController : MonoBehaviour
{
    //one sprite has 100 px per one unit
    private const int SHUFFLE_GRID_LOOPS = 3;
    private static Vector2Int CONFIG_GRID_EASY_BOUNDS = new Vector2Int(3, 5);
    private static Vector2Int CONFIG_GRID_MEDIUM_BOUNDS = new Vector2Int(4, 6);
    private static Vector2Int CONFIG_GRID_HARD_BOUNDS = new Vector2Int(5, 8);
    
    public Canvas canvasGui;
    public Text txt_title;
    public Text txt_progress;
    public Text txt_efficiency;
    public Text txt_help;
    public GameObject btn_restart;
    public GameObject btn_leave;
    public GameObject btn_help;

    private bool inHelpMode;

    private int rows;
    private int cols;

    private Sprite[] sprites_array;
    private Sprite full_image;
    private GameObject grid;
    private GameObject grid_help;
    private GameObject flames_help;
    private TileTracker[] tiles_grid;

    public int DraggedTileKey { get; set; }

    private int optimalMoves;
    private int userMoves;
    private int properlyPlacedAmount;
    private float efficiency;

    private PlayerController _playerController;
    private SceneController _sceneController;

    private Place _playedPlace;
    
    private void Awake()
    {
        _playerController = Utilities.FindPlayer();
        _sceneController = Utilities.FindSceneController();

        _playedPlace = _playerController.Places.Find(p => p.id == _playerController.CurrentPlayedPlaceId);
        InitializeGridHolder();
    }

    private void InitializeGridHolder()
    {
        Vector2Int difficulty;
        switch (_playedPlace.gameDifficulty)
        {
            case "Easy":
                difficulty = CONFIG_GRID_EASY_BOUNDS;
                break;
            case "Medium":
                difficulty = CONFIG_GRID_MEDIUM_BOUNDS;
                break;
            case "Hard":
                difficulty = CONFIG_GRID_HARD_BOUNDS;
                break;
            default:
                throw new NotImplementedException("Error: wrong difficulty");
        }
        
        InitializeGrid(_playedPlace.imagePath, _playedPlace.engName, difficulty);
        
    }

    private void InitializeGrid(string jigsaw_subject, string title, Vector2Int lvlIndicator)
    {
        txt_title.text = title;

        rows = lvlIndicator.x;
        cols = lvlIndicator.y;
        inHelpMode = false;
        txt_help.enabled = false;
        sprites_array = Resources.LoadAll<Sprite>("Sprites/" + jigsaw_subject);
        tiles_grid = new TileTracker[rows*cols];
        int key = 0;

        grid = Instantiate(Resources.Load<GameObject>("Jigsaw_prefabs/Grid_"+_playedPlace.gameDifficulty), canvasGui.transform, false);
        flames_help = Instantiate(Resources.Load<GameObject>("Jigsaw_prefabs/" + "Grid_Help_Flames"), canvasGui.transform, false);

        TileTracker[] gridChildren = grid.GetComponentsInChildren<TileTracker>();

        for (int i = 0; i < rows*cols; i++)
        {
            TileTracker tileTracker = gridChildren[key];
            tileTracker
                .SetOriginTileKey(key)
                .SetCurrentTileKey(key)
                .SetStationaryPosition(gridChildren[key].transform.localPosition)
                .SetSprite(sprites_array[key]);
            tiles_grid[key++] = tileTracker;
        }

        grid_help = Instantiate(grid, canvasGui.transform, false);
        grid_help.transform.position = flames_help.transform.position;

        ScaleGrid(grid_help, new Vector3(0.7f, 0.7f, 0.7f), false);

        grid_help.SetActive(false);
        flames_help.SetActive(false);

        InitGridForGame();
    }
    private void InitGridForGame()
    {
        properlyPlacedAmount = 0;
        DraggedTileKey = -1;
        optimalMoves = 0;
        userMoves = 0;
        ShuffleGrid();
        ComputeOptimalMoves();
        CheckProperPlacement();
    }
    
    public void MarkNewPlacement(TileTracker dragged, TileTracker on, bool isInit)
    {
        Vector3 onPos = on.GetStationaryPosition();
        Vector3 draggedPos = dragged.GetStationaryPosition();
        int onCurrentKey = on.GetcurrentTileKey();
        int draggedCurrentKey = dragged.GetcurrentTileKey();

        on.SetStationaryPosition(draggedPos);
        on.SetCurrentTileKey(draggedCurrentKey);
        dragged.SetStationaryPosition(onPos);
        dragged.SetCurrentTileKey(onCurrentKey);

        if(!isInit)
            CheckProperPlacement();
    }
    public void CheckProperPlacement()
    {
        properlyPlacedAmount = tiles_grid.Count(tileTracker => tileTracker.CheckProperPlace() && (tileTracker.CheckNeededRotationsAmount() == 0));
        UpdateProgress();

        if (properlyPlacedAmount == rows * cols)
        {
            ProceedEndGameEvent();
        }
    }
    private void ShuffleGrid()
    {
        //MarkNewPlacement(this.tiles_grid[0].RotateBy(90f), this.tiles_grid[5].RotateBy(270f),true); // this for tests and debug
        for (int i = 0; i < SHUFFLE_GRID_LOOPS * rows * cols; i++)
        {
            MarkNewPlacement(tiles_grid[UnityEngine.Random.Range(0, rows * cols)].RotateBy(UnityEngine.Random.Range(0, 4) * 90f),
            tiles_grid[UnityEngine.Random.Range(0, rows * cols)].RotateBy(UnityEngine.Random.Range(0, 4) * 90f), true);
        }
    }
    private void ComputeOptimalMoves()
    {
        foreach (TileTracker tile in tiles_grid)
        {
            if (!tile.CheckProperPlace())
                optimalMoves++;
            optimalMoves += tile.CheckNeededRotationsAmount();
        }
        optimalMoves--;
    }
    public void UpdateEfficiency()
    {
        userMoves++;
        efficiency = ((float)optimalMoves / (float)userMoves) * 100.0f;
        txt_efficiency.text = "Efficiency " + (int)efficiency + "%";
    }

    private void UpdateProgress()
    {
        txt_progress.text = "Progress " + properlyPlacedAmount + "/" + rows*cols;
    }
    
    private void PutFireOnlyOnGridBounds(TileTracker[] tilesArray)
    {
        int key = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                bool up = false, bottom = false, left = false, right = false;
                if (row == 0)
                    up = true;
                if (col == 0)
                    left = true;
                if (col == cols - 1)
                    right = true;
                if (row == rows - 1)
                    bottom = true;

                enableFlames(tilesArray[key], up, bottom, left, right);
                key++;
            }
        }
    }
    private void enableFlames(TileTracker tile, bool top, bool bottom, bool left, bool right)
    {
        ParticleSystem[] fireArray = tile.GetComponentsInChildren<ParticleSystem>();

        ParticleSystem.EmissionModule p_bottom = fireArray[0].emission;
        p_bottom.enabled = bottom;
        ParticleSystem.EmissionModule p_up = fireArray[1].emission;
        p_up.enabled = top;
        ParticleSystem.EmissionModule p_left = fireArray[2].emission;
        p_left.enabled = left;
        ParticleSystem.EmissionModule p_right = fireArray[3].emission;
        p_right.enabled = right;
    }
    private void ScaleGrid(GameObject grid, Vector3 scale, bool enableTilesFlames)
    {
        grid.transform.localScale = scale;
        if (enableTilesFlames) return;
        
        foreach (TileTracker tile in grid.GetComponentsInChildren<TileTracker>())
        {
            foreach (ParticleSystem particle in tile.GetComponentsInChildren<ParticleSystem>())
            {
                ParticleSystem.EmissionModule em = particle.emission;
                em.enabled = false;
            }
        }
    }
    private void ProceedEndGameEvent()
    {
        PutFireOnlyOnGridBounds(tiles_grid);

        Place wonPlace = _playerController.Places.Find(p => p.id == _playerController.CurrentPlayedPlaceId);
        
        _playerController.AddPoints(wonPlace.scoreValue);
        _playerController.DiscoveredPlaces.Add(wonPlace.id);
        StartCoroutine(EndGameWait(5));
    }

    private IEnumerator EndGameWait(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        _sceneController.GoToScene(SceneController.SCN_INSPIRATIONAL_LEARNING);
    }
    
    public void OnClickBtnReset()
    {
        InitGridForGame();
        if (!inHelpMode) return;
        
        DisableHelpMode();
    }
    public void OnClickBtnHelp()
    {
        if (!inHelpMode)
        {
            EnableHelpMode();
        }
        else
        {
            DisableHelpMode();
        }
    }

    private void EnableHelpMode()
    {
        btn_help.GetComponentInChildren<Text>().text = "Back to game";
        grid.SetActive(false);
        grid_help.SetActive(true);
        flames_help.SetActive(true);
        txt_title.resizeTextMaxSize = 50;
        txt_help.enabled = true;
        inHelpMode = true;
    }

    private void DisableHelpMode()
    {
        btn_help.GetComponentInChildren<Text>().text = "Help";
        grid.SetActive(true);
        grid_help.SetActive(false);
        flames_help.SetActive(false);
        txt_title.resizeTextMaxSize = 30;
        txt_help.enabled = false;
        inHelpMode = false;
    }

    public void OnClickBtnLeave()
    {
        _sceneController.GoBackFromGame();
    }
}
