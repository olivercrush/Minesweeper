using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minesweeper
{
    private bool[,] _bombs;
    private bool[,] _coverage;

    private int _width;
    private int _heigth;
    private int _bombCount;

    // Start is called before the first frame update
    public Minesweeper(int width, int height, int bombCount)
    {
        _width = width;
        _heigth = height;
        _bombCount = bombCount;

        _bombs = GenerateGrid(width, height, bombCount);
        _coverage = GenerateCoverage(width, height);
    }

    public bool[,] GetGrid()
    {
        return _bombs;
    }

    public List<(int, int, int)> DiscoverFirstCell(int x, int y)
    {
        if (_bombs[y, x])
        {
            ReplaceBomb(x, y);
        }

        return DiscoverCell(x, y, 0);
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

    public List<(int, int, int)> DiscoverCell(int x, int y, int level)
    {
        List<(int, int, int)> discoveredCells = new List<(int, int, int)>();

        if (IsValidCell(x, y))
        {
            _coverage[y, x] = true;

            int adjacentBombCount = GetAdjacentBombsCount(x, y);
            discoveredCells.Add((x, y, adjacentBombCount));

            if (adjacentBombCount == 0)
            {
                List<(int, int, int)> west = DiscoverCell(x - 1, y, 1);
                discoveredCells.AddRange(west);

                List<(int, int, int)> east = DiscoverCell(x + 1, y, 1);
                discoveredCells.AddRange(east);

                List<(int, int, int)> north = DiscoverCell(x, y - 1, 1);
                discoveredCells.AddRange(north);

                List<(int, int, int)> south = DiscoverCell(x, y + 1, 1);
                discoveredCells.AddRange(south);

                List<(int, int, int)> northwest = DiscoverCell(x - 1, y + 1, 1);
                discoveredCells.AddRange(northwest);

                List<(int, int, int)> northeast = DiscoverCell(x + 1, y + 1, 1);
                discoveredCells.AddRange(northeast);

                List<(int, int, int)> southwest = DiscoverCell(x - 1, y - 1, 1);
                discoveredCells.AddRange(southwest);

                List<(int, int, int)> southeast = DiscoverCell(x + 1, y - 1, 1);
                discoveredCells.AddRange(southeast);
            }
        }
        else if (level == 0 && _bombs[y, x])
        {
            discoveredCells.Add((x, y, -1));
        }

        return discoveredCells;
    }

    private bool IsValidCell(int x, int y)
    {
        return y >= 0 && y < _heigth && x >= 0 && x < _width && !_coverage[y, x] && !_bombs[y, x];
    }

    private int GetAdjacentBombsCount(int x, int y)
    {
        int count = 0;

        for (int i = y - 1; i <= y + 1; i++) {
            for (int j = x - 1; j <= x + 1; j++) {
                if (i >= 0 && i < _heigth && j >= 0 && j < _width && !(i == y && j == x) && _bombs[i, j])
                {
                    count++;
                }
            }
        }

        return count;
    }

    private bool[,] GenerateGrid(int width, int heigth, int bombCount)
    {
        bool[,] grid = new bool[heigth, width];
        
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
                y = Mathf.FloorToInt(Random.value * heigth);
            } while (grid[y,x]);
            grid[y,x] = true;
            //Debug.Log("Bomb at (" + x + ";" + y + ")");
        }

        return grid;
    }

    private bool[,] GenerateCoverage(int width, int height)
    {
        bool[,] coverage = new bool[height, width];

        for (int i = 0; i < coverage.GetLength(0); i++) {
            for (int j = 0; j < coverage.GetLength(1); j++) {
                coverage[i, j] = false;
            }
        }

        return coverage;
    }
}
