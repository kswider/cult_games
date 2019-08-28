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

    private string lvl_indicator;
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

    private int rows;
    private int cols;

    private Sprite[] sprites_array;
    private Sprite full_image;
    private GameObject grid;
    private GameObject grid_help;
    private GameObject flames_help;
    private TileTracker[] tiles_grid;

    private int draggedTileKey;

    private int optimalMoves;
    private int userMoves;
    private int properlyPlacedAmount;
    private float efficiency;

    void Awake()
    {
        InitializeGridHolder(1);
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
        this.txt_title.text = title;
        switch (lvlIndicator.x)
        {
            case 3:
                this.lvl_indicator = "Grid_Easy";
                break;
            case 4:
                this.lvl_indicator = "Grid_Medium";
                break;
            case 5:
                this.lvl_indicator = "Grid_Hard";
                break;
        }
        this.successfull = false;
        this.rows = lvlIndicator.x;
        this.cols = lvlIndicator.y;
        this.inHelpMode = false;
        this.txt_help.enabled = false;
        this.sprites_array = Resources.LoadAll<Sprite>("Sprites/" + jigsaw_subject);
        this.tiles_grid = new TileTracker[rows*cols];
        int key = 0;

        this.grid = Instantiate(Resources.Load<GameObject>("Jigsaw_prefabs/"+lvl_indicator));
        this.flames_help = Instantiate(Resources.Load<GameObject>("Jigsaw_prefabs/" + "Grid_Help_Flames"));

        grid.transform.SetParent(canvasGui.transform, false);
        TileTracker[] gridChildren = grid.GetComponentsInChildren<TileTracker>();

        for (int i = 0; i < rows*cols; i++)
        {
            TileTracker tileTracker = gridChildren[key];
          
            tileTracker.SetOriginTileKey(key).SetCurrentTileKey(key).SetStationaryPosition(gridChildren[key].transform.localPosition).SetSprite(this.sprites_array[key]);
            this.tiles_grid[key++] = tileTracker;
        }

        this.grid_help = Instantiate(this.grid);
        this.grid_help.transform.position = this.flames_help.transform.position;

        this.grid_help.transform.SetParent(canvasGui.transform, false);
        this.flames_help.transform.SetParent(canvasGui.transform, false);

        this.ScaleGrid(this.grid_help, new Vector3(0.7f, 0.7f, 0.7f), false);

        this.grid_help.SetActive(false);
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
        foreach(TileTracker tileTracker in this.tiles_grid)
        {
            if (tileTracker.CheckProperPlace() && (tileTracker.CheckNeededRotationsAmount() == 0))
            {
                properlyPlacedAmount++;
            }
        }
        this.properlyPlacedAmount = properlyPlacedAmount;
        this.updateProgress();

        if (properlyPlacedAmount == rows * cols)
        {
            proceedEndGameEvent();
        }
    }
    private void ShuffleGrid()
    {
        //MarkNewPlacement(this.tiles_grid[0].RotateBy(90f), this.tiles_grid[5].RotateBy(270f),true); // this for tests and debug
        for (int i = 0; i < SHUFFLE_GRID_LOOPS * rows * cols; i++)
        {
            MarkNewPlacement(this.tiles_grid[UnityEngine.Random.Range(0, rows * cols)].RotateBy(UnityEngine.Random.Range(0, 4) * 90f),
            this.tiles_grid[UnityEngine.Random.Range(0, rows * cols)].RotateBy(UnityEngine.Random.Range(0, 4) * 90f), true);
        }
    }
    private void computeOptimalMoves()
    {
        foreach (TileTracker tile in this.tiles_grid)
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
        this.txt_progress.text = "Progress " + this.properlyPlacedAmount + "/" + this.rows*this.cols;
    }
    public void updateTitle(String title)
    {
        this.txt_title.text = title;
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

                this.enableFlames(tilesArray[key], up, bottom, left, right);
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
        if (!enableTilesFlames)
        {
            foreach (TileTracker tile in grid.GetComponentsInChildren<TileTracker>())
            {
                foreach (ParticleSystem particle in tile.GetComponentsInChildren<ParticleSystem>())
                {
                    ParticleSystem.EmissionModule em = particle.emission;
                    em.enabled = false;
                }
            }
        }
    }
    private void proceedEndGameEvent()
    {
        this.PutFireOnlyOnGridBounds(this.tiles_grid);
        this.successfull = true;
    }
    public void OnClickBtnReset()
    {
        initGridForGame();
        if (this.inHelpMode)
        {
            
            this.btn_help.GetComponentInChildren<Text>().text = "Help";
            this.inHelpMode = false;
            this.grid.SetActive(true);
            this.grid_help.SetActive(false);
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
            this.grid.SetActive(false);
            this.grid_help.SetActive(true);
            this.flames_help.SetActive(true);
            this.txt_title.resizeTextMaxSize = 50;
            this.txt_help.enabled = true;
        }
        else
        {
            this.btn_help.GetComponentInChildren<Text>().text = "Help";
            this.inHelpMode = false;
            this.grid.SetActive(true);
            this.grid_help.SetActive(false);
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
