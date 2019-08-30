using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridController : MonoBehaviour
{
    //one sprite has 100 px per one unit
    private static int SHUFFLE_GRID_LOOPS = 3;
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
    private bool successfull;

    private JigsawGrid grid;
    private JigsawGrid grid_help;
    private GameObject flames_help;

    private int draggedTileKey;

    private int optimalMoves;
    private int userMoves;
    private int properlyPlacedAmount;
    private float efficiency;

    void Awake()
    {
        InitializeGridHolder(3);
    }

    public void InitializeGridHolder(int id)
    {
        switch (id)
        {
            case 1:
                InitializeGrid("mariacki_inside_lvl_hard", "St. Mary's Basilica", CONFIG_GRID_HARD_BOUNDS);
                break;
            case 11:
                InitializeGrid("mariacki_outside_lvl_easy", "St. Mary's Basilica", CONFIG_GRID_EASY_BOUNDS);
                break;
            case 2:
                InitializeGrid("sukiennice_lvl_hard", "Krakow Cloth Hall", CONFIG_GRID_HARD_BOUNDS);
                break;
            case 3:
                InitializeGrid("wawel_lvl_med", "Wawel Castle", CONFIG_GRID_MEDIUM_BOUNDS);
                break;
            case 202:
                InitializeGrid("king_krak_lvl_med", "The Legend of Krakus", CONFIG_GRID_MEDIUM_BOUNDS);
                break;
            case 201:
                InitializeGrid("wawel_dragon_lvl_med", "Wawel Dragon", CONFIG_GRID_MEDIUM_BOUNDS);
                break;
            case 203:
                InitializeGrid("wandas_death_lvl_hard", "The legend of Wanda", CONFIG_GRID_HARD_BOUNDS);
                break;
            case 101:
                InitializeGrid("rakowicki_lvl_easy", "Rakowicki Cemetery", CONFIG_GRID_EASY_BOUNDS);
                break;
            case 102:
                InitializeGrid("kosciuszko_mound_lvl_easy", "Kosciuszko Mound", CONFIG_GRID_EASY_BOUNDS);
                break;
            case 103:
                InitializeGrid("ice_lvl_easy", "ICE Congress Centre", CONFIG_GRID_EASY_BOUNDS);
                break;
            case 4:
                InitializeGrid("uj_garden_lvl_med", "Botanic Garden of the UJ", CONFIG_GRID_MEDIUM_BOUNDS);
                break;
        }
    }

    private void InitializeGrid(String jigsaw_subject, string title, Vector2Int lvlIndicator)
    {
        this.grid = new JigsawGrid(lvlIndicator, jigsaw_subject, this.canvasGui);
        this.grid_help = new JigsawGrid(lvlIndicator, jigsaw_subject, this.canvasGui);

        this.txt_title.text = title; 
        this.successfull = false;
        this.inHelpMode = false;
        this.txt_help.enabled = false;
        
        this.flames_help = Instantiate(Resources.Load<GameObject>("Jigsaw_prefabs/" + "Grid_Help_Flames"));
        this.flames_help.transform.SetParent(canvasGui.transform, false);

        GridHelper.SetPosition(this.grid_help, this.flames_help.transform);
        GridHelper.ScaleGrid(this.grid_help, new Vector3(0.7f, 0.7f, 0.7f));
        GridHelper.TurnOffFlamesAtAll(this.grid_help);
        GridHelper.TurnOffPhysics(this.grid_help);

        GridHelper.SetGravityModifierForFlames(this.grid, -0.02f);

        this.grid_help.gameObjectGrid.SetActive(false);
        this.flames_help.SetActive(false);

        initGridForGame();
    }
    private void initGridForGame()
    {
        this.properlyPlacedAmount = 0;
        this.draggedTileKey = -1;
        this.optimalMoves = 0;
        this.userMoves = 0;
        this.ShuffleGrid();
        this.computeOptimalMoves();
        this.CheckProperPlacement();
    }

    public void SetDraggingTile(int key)
    {
        this.draggedTileKey = key;
    }
    public int CheckDraggingTile()
    {
        return this.draggedTileKey;
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
        int properlyPlacedAmount = 0;
        foreach(TileTracker tileTracker in this.grid.tiles_grid)
        {
            if (tileTracker.CheckProperPlace() && (tileTracker.CheckNeededRotationsAmount() == 0))
            {
                properlyPlacedAmount++;
            }
        }
        this.properlyPlacedAmount = properlyPlacedAmount;
        this.updateProgress();

        if (properlyPlacedAmount == this.grid.rows * this.grid.cols)
        {
            proceedEndGameEvent();
        }
    }
    private void ShuffleGrid()
    {
        //MarkNewPlacement(this.grid.tiles_grid[0].RotateBy(90f), this.grid.tiles_grid[5].RotateBy(270f), true); // this for tests and debug
        for (int i = 0; i < SHUFFLE_GRID_LOOPS * this.grid.rows * this.grid.cols; i++)
        {
            MarkNewPlacement(this.grid.tiles_grid[UnityEngine.Random.Range(0, this.grid.rows * this.grid.cols)].RotateBy(UnityEngine.Random.Range(0, 4) * 90f),
            this.grid.tiles_grid[UnityEngine.Random.Range(0, this.grid.rows * this.grid.cols)].RotateBy(UnityEngine.Random.Range(0, 4) * 90f), true);
        }
    }
    private void computeOptimalMoves()
    {
        foreach (TileTracker tile in this.grid.tiles_grid)
        {
            if (!tile.CheckProperPlace())
                this.optimalMoves++;
            this.optimalMoves += tile.CheckNeededRotationsAmount();
        }
        this.optimalMoves--;
    }
    public void updateEfficiency()
    {
        this.userMoves++;
        this.efficiency = ((float)this.optimalMoves / (float)this.userMoves) * 100.0f;
        this.txt_efficiency.text = "Efficiency " + (int)(this.efficiency)+ "%";
    }
    public void updateProgress()
    {
        this.txt_progress.text = "Progress " + this.properlyPlacedAmount + "/" + this.grid.rows*this.grid.cols;
    }
    public void updateTitle(String title)
    {
        this.txt_title.text = title;
    }
   
    private void proceedEndGameEvent()
    {
        GridHelper.PutFireOnlyOnGridBounds(this.grid);
        this.successfull = true;
    }
    public void OnClickBtnReset()
    {
        initGridForGame();
        if (this.inHelpMode)
        {
            
            this.btn_help.GetComponentInChildren<Text>().text = "Help";
            this.inHelpMode = false;
            this.grid.gameObjectGrid.SetActive(true);
            this.grid_help.gameObjectGrid.SetActive(false);
            this.flames_help.SetActive(false);
            this.txt_title.resizeTextMaxSize = 30;
            this.txt_help.enabled = false;
            
        }
    }
    public void OnClickBtnHelp()
    {
        if (!this.inHelpMode)
        {
            this.btn_help.GetComponentInChildren<Text>().text = "Back to game";
            this.inHelpMode = true;
            this.grid.gameObjectGrid.SetActive(false);
            this.grid_help.gameObjectGrid.SetActive(true);
            this.flames_help.SetActive(true);
            this.txt_title.resizeTextMaxSize = 50;
            this.txt_help.enabled = true;
        }
        else
        {
            this.btn_help.GetComponentInChildren<Text>().text = "Help";
            this.inHelpMode = false;
            this.grid.gameObjectGrid.SetActive(true);
            this.grid_help.gameObjectGrid.SetActive(false);
            this.flames_help.SetActive(false);
            this.txt_title.resizeTextMaxSize = 30;
            this.txt_help.enabled = false;
        }

    }

    public void OnClickBtnLeave()
    {
        //
    }
}
