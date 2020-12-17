using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minesweeper
{
    private bool[,] _grid;

    private int _width;
    private int _heigth;
    private int _bombCount;

    // Start is called before the first frame update
    public Minesweeper(int width, int height, int bombCount)
    {
        _width = width;
        _heigth = height;
        _bombCount = bombCount;

        _grid = GenerateGrid(width, height, bombCount);
    }

    public bool[,] GetGrid()
    {
        return _grid;
    }

    public (bool, int) DiscoverCell(int x, int y)
    {
        // bombed, number of neighbors
        return (_grid[y, x], 0);
    }

    private bool[,] GenerateGrid(int width, int height, int bombCount)
    {
        bool[,] grid = new bool[height, width];
        
        for (int i = 0; i < grid.GetLength(0); i++) {
            for (int j = 0; j < grid.GetLength(1); j++) {
                grid[i, j] = false;
            }
        }

        for (int i = 0; i < _bombCount; i++) {
            int x, y;
            do
            {
                x = Mathf.FloorToInt(Random.value * width);
                y = Mathf.FloorToInt(Random.value * height);
            } while (grid[y,x]);
            grid[y,x] = true;
            //Debug.Log("Bomb at (" + x + ";" + y + ")");
        }

        return grid;
    }
}
