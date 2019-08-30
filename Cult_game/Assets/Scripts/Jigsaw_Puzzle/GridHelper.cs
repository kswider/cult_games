using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridHelper
{
    public static void SetPosition(JigsawGrid grid, Transform transform)
    {
        grid.gameObjectGrid.transform.position = transform.position;
    }
    public static void ScaleGrid(JigsawGrid grid, Vector3 scale)
    {
        grid.gameObjectGrid.transform.localScale = scale;

    }
    public static void TurnOffFlamesAtAll(JigsawGrid grid)
    {
        foreach (TileTracker tile in  grid.tiles_grid)
        {
            foreach (ParticleSystem particle in tile.GetComponentsInChildren<ParticleSystem>())
            {
                ParticleSystem.EmissionModule em = particle.emission;
                em.enabled = false;
            }
        }
    }
    public static void TurnOffPhysics(JigsawGrid grid)   //Grid is not clickable
    {
        grid.gameObjectGrid.GetComponent<EdgeCollider2D>().isTrigger = false;
        foreach (TileTracker tile in grid.gameObjectGrid.GetComponentsInChildren<TileTracker>())
        {
            tile.GetComponent<Rigidbody2D>().simulated = false;
        }
    }
    public static void PutFireOnlyOnGridBounds(JigsawGrid grid) //  wanna some fire only on frame? help yourself!
    {
        int key = 0;
        for (int row = 0; row < grid.rows; row++)
        {
            for (int col = 0; col < grid.cols; col++)
            {
                bool up = false, bottom = false, left = false, right = false;
                if (row == 0)
                    up = true;
                if (col == 0)
                    left = true;
                if (col == grid.cols - 1)
                    right = true;
                if (row == grid.rows - 1)
                    bottom = true;

                EnableFlames(grid.tiles_grid[key], up, bottom, left, right);
                key++;
            }
        }
    }
    public static void EnableFlames(TileTracker tile, bool top, bool bottom, bool left, bool right)
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
    } //method for setting particular tile edges on fire

    public static void SetGravityModifierForFlames(JigsawGrid grid, float modifier) //DEFAULT - 0.03, BE CAREFUL WITH HIGHER!!!
    {
        foreach (TileTracker tile in grid.tiles_grid)
        {
            foreach (ParticleSystem particle in tile.GetComponentsInChildren<ParticleSystem>())
            {
                ParticleSystem.MainModule main = particle.main;
                main.gravityModifier = modifier;
            }
        }
    }



}
