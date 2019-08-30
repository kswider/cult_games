using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigsawGrid
{
    public GameObject gameObjectGrid;
    public Canvas parentCanvas; // should be gui
    public Sprite[] sprites_array;
    public int rows;
    public int cols;
    public TileTracker[] tiles_grid;

    public string lvl_indicator;
    public string jigsaw_subject;

    public JigsawGrid(Vector2Int lvlIndicator, string jigsaw_subject, string gameDifficulty, Canvas parentCanvas)
    {
        this.parentCanvas = parentCanvas;
        this.rows = lvlIndicator.y;
        this.cols = lvlIndicator.x;

        this.sprites_array = Resources.LoadAll<Sprite>("Sprites/" + jigsaw_subject);
        this.tiles_grid = new TileTracker[rows * cols];

        this.gameObjectGrid = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Jigsaw_prefabs/Grid_" + gameDifficulty), parentCanvas.transform, false);


        TileTracker[] gridChildren = gameObjectGrid.GetComponentsInChildren<TileTracker>();

        int key = 0;
        for (int i = 0; i < rows * cols; i++)
        {
            TileTracker tileTracker = gridChildren[key];
            tileTracker
                .SetOriginTileKey(key)
                .SetCurrentTileKey(key)
                .SetStationaryPosition(gridChildren[key].transform.localPosition)
                .SetSprite(sprites_array[key]);
            tiles_grid[key++] = tileTracker;
        }
    }
}
