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
    public static Vector2Int CONFIG_GRID_EASY_BOUNDS = new Vector2Int(3, 5);
    public static Vector2Int CONFIG_GRID_MEDIUM_BOUNDS = new Vector2Int(4, 6);
    public static Vector2Int CONFIG_GRID_HARD_BOUNDS = new Vector2Int(5, 8);

    public Canvas canvasGui;
    public Text txt_title;
    public Text txt_progress;
    public Text txt_efficiency;
    public Text txt_help;
    public GameObject btn_restart;
    public GameObject btn_leave;
    public GameObject btn_help;

    private bool inHelpMode;

    private JigsawGrid grid;
    private JigsawGrid grid_help;
    private GameObject flames_help;

    public int DraggedTileKey { get; set; }

    private int optimalMoves;
    private int userMoves;
    private int properlyPlacedAmount;
    private float efficiency;

    private PlayerController _playerController;
    private SceneController _sceneController;

    private Place _playedPlace;

    public static Vector2Int DifficultyToBounds(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                return CONFIG_GRID_EASY_BOUNDS;
            case "Medium":
                return CONFIG_GRID_MEDIUM_BOUNDS;
            case "Hard":
                return CONFIG_GRID_HARD_BOUNDS;
            default:
                throw new NotImplementedException("Error: wrong difficulty");
        }
    }
    
    private void Awake()
    {
        _playerController = Utilities.FindPlayerController();
        _sceneController = Utilities.FindSceneController();

        _playedPlace = _playerController.Places.Find(p => p.id == _playerController.CurrentPlayedPlaceId);
        InitializeGridHolder();
    }

    private void InitializeGridHolder()
    {
        Vector2Int difficulty = DifficultyToBounds(_playedPlace.gameDifficulty);
        
        InitializeGrid(_playedPlace.imagePath, _playedPlace.engName, difficulty);
    }

    private void InitializeGrid(string jigsaw_subject, string title, Vector2Int lvlIndicator)
    {
        grid = new JigsawGrid(lvlIndicator, jigsaw_subject, _playedPlace.gameDifficulty, canvasGui);
        grid_help = new JigsawGrid(lvlIndicator, jigsaw_subject, _playedPlace.gameDifficulty, canvasGui);

        txt_title.text = title; 
        inHelpMode = false;
        txt_help.enabled = false;
        
        flames_help = Instantiate(Resources.Load<GameObject>("Jigsaw_prefabs/" + "Grid_Help_Flames"), canvasGui.transform, false);

        GridHelper.SetPosition(grid_help, flames_help.transform);
        GridHelper.ScaleGrid(grid_help, new Vector3(0.7f, 0.7f, 0.7f));
        GridHelper.TurnOffFlamesAtAll(grid_help);
        GridHelper.TurnOffPhysics(grid_help);

        GridHelper.SetGravityModifierForFlames(grid, -0.02f);
        
        grid_help.gameObjectGrid.SetActive(false);
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
        properlyPlacedAmount = grid.tiles_grid.Count(tileTracker => tileTracker.CheckProperPlace() && (tileTracker.CheckNeededRotationsAmount() == 0));
        UpdateProgress();

        if (properlyPlacedAmount == grid.rows * grid.cols)
        {
            ProceedEndGameEvent();
        }
    }
    private void ShuffleGrid()
    {
        //MarkNewPlacement(this.grid.tiles_grid[0].RotateBy(90f), this.grid.tiles_grid[5].RotateBy(270f), true); // this for tests and debug
        for (int i = 0; i < SHUFFLE_GRID_LOOPS * grid.rows * grid.cols; i++)
        {
            MarkNewPlacement(grid.tiles_grid[UnityEngine.Random.Range(0, grid.rows * grid.cols)].RotateBy(UnityEngine.Random.Range(0, 4) * 90f),
            grid.tiles_grid[UnityEngine.Random.Range(0, grid.rows * grid.cols)].RotateBy(UnityEngine.Random.Range(0, 4) * 90f), true);
        }
    }
    private void ComputeOptimalMoves()
    {
        foreach (TileTracker tile in grid.tiles_grid)
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
        txt_progress.text = "Progress " + properlyPlacedAmount + "/" + grid.rows* grid.cols;
    }

    private void ProceedEndGameEvent()
    {
        GridHelper.PutFireOnlyOnGridBounds(grid);

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
        grid.gameObjectGrid.SetActive(false);
        grid_help.gameObjectGrid.SetActive(true);
        flames_help.SetActive(true);
        txt_title.resizeTextMaxSize = 50;
        txt_help.enabled = true;
        inHelpMode = true;
    }

    private void DisableHelpMode()
    {
        btn_help.GetComponentInChildren<Text>().text = "Help";
        grid.gameObjectGrid.SetActive(true);
        grid_help.gameObjectGrid.SetActive(false);
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
