using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minesweeper
{
    private MinesweeperGrid _grid;

    // Start is called before the first frame update
    public Minesweeper(int width, int height, int bombCount)
    {
        _grid = new MinesweeperGrid(width, height, bombCount);
    }

    public bool[,] GetBombs()
    {
        return _grid.GetBombs();
    }

    public List<(int, int, int)> DiscoverFirstCell(int x, int y)
    {
        if (_grid.GetBombs()[y, x])
        {
            _grid.ReplaceBomb(x, y);
        }

        return DiscoverCell(x, y, 0);
    }

    public List<(int, int, int)> DiscoverCell(int x, int y, int level)
    {
        List<(int, int, int)> discoveredCells = new List<(int, int, int)>();

        if (_grid.IsValidCell(x, y))
        {
            _grid.CoverCell(x, y);

            int adjacentBombCount = _grid.GetAdjacentBombsCount(x, y);
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
        else if (level == 0 && _grid.GetBombs()[y, x])
        {
            discoveredCells.Add((x, y, 9));
        }

        return discoveredCells;
    }

    
}
