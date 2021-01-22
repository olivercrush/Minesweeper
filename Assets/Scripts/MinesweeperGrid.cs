using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesweeperGrid
{
    private Cell[,] _grid;

    private int _width;
    private int _heigth;
    private int _bombCount;

    public MinesweeperGrid(int width, int height, int bombCount)
    {
        _width = width;
        _heigth = height;
        _bombCount = bombCount;

        _grid = GenerateGrid(width, height, bombCount);
    }

    public void TurnCell(int x, int y)
    {
        _grid[y, x].TurnCell();
    }

    public void ReplaceBomb(int x, int y)
    {
        if (_grid[y, x].IsBomb())
        {
            _grid[y, x].SetBomb(false);

            int newX;
            int newY;

            do
            {
                newX = Mathf.FloorToInt(Random.value * _width);
                newY = Mathf.FloorToInt(Random.value * _heigth);
            } while (_grid[newY, newX].IsBomb());

            _grid[newY, newX].SetBomb(true);
        }
        else
        {
            Debug.LogWarning("[MinesweeperGrid].ReplaceBomb(int x, int y) : Tried to replace a non-bomb cell");
        }
    }

    public void ReplaceBomb(Cell bomb)
    {
        if (bomb.IsBomb())
        {
            (int, int) coords = bomb.GetCoordinates();
            ReplaceBomb(coords.Item1, coords.Item2);
        }
    }

    /// <summary>
    /// method <c>IsValidCell</c> checks if the coordinates are contained in the grid domain
    /// </summary>
    /// <param name="x">the x position of the point to check</param>
    /// <param name="y">the y position of the point to check</param>
    /// <returns>true if the point is in the grid domain, false otherwise</returns>
    public bool IsValidCell(int x, int y)
    {
        return y >= 0 && y < _heigth && x >= 0 && x < _width;
    }

    /// <summary>
    /// method <c>GetAdjacentBombCount</c> returns the number of bombs in adjacent cells
    /// </summary>
    /// <param name="x">the x position of the cell</param>
    /// <param name="y">the y position of the cell</param>
    /// <returns>an integer that represents the number of bombs in adjacent cells (max. 8)</returns>
    public int GetAdjacentBombCount(int x, int y)
    {
        int count = 0;

        // TODO : make this verification in upper class
        if (_grid[y, x].IsBomb()) return 9;

        List<Cell> adjacentCells = GetAdjacentCells(x, y);
        foreach (Cell cell in adjacentCells)
        {
            if (cell.IsBomb()) count++;
        }

        return count;
    }

    /// <summary>
    /// method <c>GetBombsInRange</c> returns all bomb cells in range from (x, y), using taxi-distance
    /// </summary>
    /// <param name="x">the x position of the cell</param>
    /// <param name="y">the y position of the cell</param>
    /// <param name="range">the range in which we want to detect bombs</param>
    /// <returns>the list containing all bomb cells in range of (x, y)</returns>
    public List<Cell> GetBombsInRange(int x, int y, int range)
    {
        List<Cell> bombList = new List<Cell>();

        for (int i = y - range; i <= y + range; i++)
        {
            for (int j = x - range; j <= x + range; j++)
            {
                if (IsValidCell(j, i) && (Mathf.Abs(i - y) + Mathf.Abs(j - x) <= range) && _grid[i, j].IsBomb())
                {
                    bombList.Add(_grid[i, j]);
                }
            }
        }

        return bombList;
    }

    public List<Cell> GetAdjacentCells(int x, int y)
    {
        List<Cell> adjacentCells = new List<Cell>();

        for (int i = y - 1; i <= y + 1; i++)
        {
            for (int j = x - 1; j <= x + 1; j++)
            {
                if (IsValidCell(j, i) && !(j == x && i == y))
                {
                    adjacentCells.Add(_grid[i, j]);
                }
            }
        }

        return adjacentCells;
    }

    private Cell[,] GenerateGrid(int width, int heigth, int bombCount)
    {
        Cell[,] grid = new Cell[heigth, width];

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = new Cell(j, i, false);
            }
        }

        for (int i = 0; i < _bombCount; i++)
        {
            int x, y;

            do
            {
                x = Mathf.FloorToInt(Random.value * width);
                y = Mathf.FloorToInt(Random.value * heigth);
            } while (grid[y, x].IsBomb());

            grid[y, x].SetBomb(true);
        }

        return grid;
    }

    // GETTERS

    public bool IsBomb(int x, int y)
    {
        return _grid[y, x].IsBomb();
    }

    public bool IsTurned(int x, int y)
    {
        return _grid[y, x].IsTurned();
    }
}
