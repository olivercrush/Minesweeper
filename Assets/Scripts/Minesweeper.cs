using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minesweeper : IObservable
{
    private MinesweeperGrid _grid;
    private List<IObserver> _observers;
    private List<(int, int, int)> _updatedCells;

    public Minesweeper(int width, int height, int bombCount)
    {
        _grid = new MinesweeperGrid(width, height, bombCount);
        _observers = new List<IObserver>();
        _updatedCells = new List<(int, int, int)>();
    }

    public List<(int, int, int)> GetUpdatedCells()
    {
        return _updatedCells;
    }

    public void DiscoverFirstCell(int x, int y)
    {
        List<Cell> bombList;

        // TODO : find better way to prevent replacing in range

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

        DiscoverCell(x, y, 0);
    }

    public void DiscoverCell(int x, int y, int level = 0)
    {
        if (_grid.IsValidCell(x, y) && !_grid.IsBomb(x, y) && !_grid.IsTurned(x, y))
        {
            _grid.TurnCell(x, y);

            int adjacentBombCount = _grid.GetAdjacentBombCount(x, y);
            _updatedCells.Add((x, y, adjacentBombCount));

            // if there is no adjacent bombs, we turn all adjacent cells
            if (adjacentBombCount == 0)
            {
                List<Cell> adjacentCells = _grid.GetAdjacentCells(x, y);
                foreach (Cell cell in adjacentCells)
                {
                    (int, int) coords = cell.GetCoordinates();
                    if (!_grid.IsTurned(coords.Item1, coords.Item2))
                    {
                        DiscoverCell(coords.Item1, coords.Item2, 1);
                    }
                }
            }
        }
        else if (level == 0 && _grid.IsBomb(x, y))
        {
            _updatedCells.Add((x, y, 9));
        }

        if (level == 0) NotifyObservers();
    }

    public void RegisterObserver(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void NotifyObservers()
    {
        //Debug.Log(_updatedCells.Count + " updates");

        foreach (IObserver o in _observers)
        {
            o.UpdateFromObservable();
        }

        _updatedCells.Clear();
    }
}
