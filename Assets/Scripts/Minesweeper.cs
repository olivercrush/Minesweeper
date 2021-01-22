using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minesweeper
{
    private MinesweeperGrid _grid;

    public Minesweeper(int width, int height, int bombCount)
    {
        _grid = new MinesweeperGrid(width, height, bombCount);
    }

    public List<(int, int, int)> DiscoverFirstCell(int x, int y)
    {
        List<Cell> bombList;

        // find better way to prevent replacing in range

        do
        {
            bombList = _grid.GetBombsInRange(x, y, 2);
            if (bombList.Count > 0)
            {
                foreach (Cell bomb in bombList)
                {
                    _grid.ReplaceBomb(bomb);
                }
            }
        } while (bombList.Count > 0);

        return DiscoverCell(x, y, 0);
    }

    public List<(int, int, int)> DiscoverCell(int x, int y, int level = 0)
    {
        List<(int, int, int)> turnedCells = new List<(int, int, int)>();

        if (_grid.IsValidCell(x, y) && !_grid.IsBomb(x, y) && !_grid.IsTurned(x, y))
        {
            _grid.TurnCell(x, y);

            int adjacentBombCount = _grid.GetAdjacentBombCount(x, y);
            turnedCells.Add((x, y, adjacentBombCount));

            // s'il n'y a pas de bombes aux alentours, on tourne également les cellules adjacentes
            if (adjacentBombCount == 0)
            {
                List<Cell> adjacentCells = _grid.GetAdjacentCells(x, y);
                foreach (Cell cell in adjacentCells)
                {
                    (int, int) coords = cell.GetCoordinates();
                    if (!_grid.IsTurned(coords.Item1, coords.Item2))
                    {
                        List<(int, int, int)> tmp = DiscoverCell(coords.Item1, coords.Item2, 1);
                        turnedCells.AddRange(tmp);
                    }
                }
            }
        }
        else if (level == 0 && _grid.IsBomb(x, y))
        {
            turnedCells.Add((x, y, 9));
        }

        return turnedCells;
    }

    
}
