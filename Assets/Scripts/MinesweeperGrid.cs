using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesweeperGrid
{
    private bool[,] _bombs;
    private bool[,] _coverage;

    private int _width;
    private int _heigth;
    private int _bombCount;

    public MinesweeperGrid(int width, int height, int bombCount)
    {
        _width = width;
        _heigth = height;
        _bombCount = bombCount;

        _bombs = GenerateGrid(width, height, bombCount);
        _coverage = GenerateCoverage(width, height);
    }

    public bool[,] GetBombs()
    {
        return _bombs;
    }

    public bool[,] GetCoverage()
    {
        return _coverage;
    }

    public void CoverCell(int x, int y)
    {
        _coverage[y, x] = true;
    }

    public void ReplaceBomb(int x, int y)
    {
        _bombs[y, x] = false;

        int newX;
        int newY;

        do
        {
            newX = Mathf.FloorToInt(Random.value * _width);
            newY = Mathf.FloorToInt(Random.value * _heigth);
        } while (_bombs[newY, newX]);

        _bombs[newY, newX] = true;
    }

    public bool IsValidCell(int x, int y)
    {
        return y >= 0 && y < _heigth && x >= 0 && x < _width && !_coverage[y, x];
    }

    public int GetAdjacentBombsCount(int x, int y)
    {
        int count = 0;

        for (int i = y - 1; i <= y + 1; i++)
        {
            for (int j = x - 1; j <= x + 1; j++)
            {
                if (i >= 0 && i < _heigth && j >= 0 && j < _width && !(i == y && j == x) && _bombs[i, j])
                {
                    count++;
                }
            }
        }

        return count;
    }

    public List<(int, int)> GetBombsInRange(int x, int y, int range)
    {
        List<(int, int)> bombList = new List<(int, int)>();

        for (int i = y - range; i <= y + range; i++)
        {
            for (int j = x - range; j <= x + range; j++)
            {
                if (IsValidCell(j, i) && _bombs[i, j])
                {
                    bombList.Add((j, i));
                }
            }
        }

        return bombList;
    }

    private bool[,] GenerateGrid(int width, int heigth, int bombCount)
    {
        bool[,] grid = new bool[heigth, width];

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = false;
            }
        }

        for (int i = 0; i < _bombCount; i++)
        {
            int x, y;
            do
            {
                x = Mathf.FloorToInt(Random.value * width);
                y = Mathf.FloorToInt(Random.value * heigth);
            } while (grid[y, x]);
            grid[y, x] = true;
            //Debug.Log("Bomb at (" + x + ";" + y + ")");
        }

        return grid;
    }

    private bool[,] GenerateCoverage(int width, int height)
    {
        bool[,] coverage = new bool[height, width];

        for (int i = 0; i < coverage.GetLength(0); i++)
        {
            for (int j = 0; j < coverage.GetLength(1); j++)
            {
                coverage[i, j] = false;
            }
        }

        return coverage;
    }
}
