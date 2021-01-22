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

    public bool IsBomb(int x, int y)
    {
        return _grid[y, x].IsBomb();
    }

    public bool IsTurned(int x, int y)
    {
        return _grid[y, x].IsTurned();
    }

    public void CoverCell(int x, int y)
    {
        _grid[y, x].TurnCell();
    }

    public void ReplaceBomb(int x, int y)
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

    public bool IsValidCell(int x, int y)
    {
        return y >= 0 && y < _heigth && x >= 0 && x < _width /*&& !_grid[y, x].IsTurned()*/;
    }

    public int GetAdjacentBombCount(int x, int y)
    {
        int count = 0;

        if (_grid[y, x].IsBomb()) return 9;

        List<Cell> adjacentCells = GetAdjacentCells(x, y);
        foreach (Cell cell in adjacentCells)
        {
            if (cell.IsBomb()) count++;
        }

        return count;
    }

    public List<(int, int)> GetBombCountInRange(int x, int y, int range)
    {
        List<(int, int)> bombList = new List<(int, int)>();

        for (int i = y - range; i <= y + range; i++)
        {
            for (int j = x - range; j <= x + range; j++)
            {
                if (IsValidCell(j, i) && _grid[i, j].IsBomb())
                {
                    bombList.Add((j, i));
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

        Debug.Log(adjacentCells.Count);
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
}
