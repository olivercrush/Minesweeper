using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO :
// [2] Find better way to prevent replacing in range

public class Minesweeper : IObservable
{
    private MinesweeperGrid _grid;
    private List<IObserver> _observers;

    private List<CellUpdate> _updatedCells;

    public struct CellUpdate
    {
        public CellUpdateType type;
        public int x;
        public int y;
        public int moore_bombs_count;

        public CellUpdate(CellUpdateType t, int x, int y, int m)
        {
            this.type = t;
            this.x = x;
            this.y = y;
            this.moore_bombs_count = m;
        }
    }

    public enum CellUpdateType
    {
        DISCOVERED,
        MARKED,
        UNMARKED,
        EXPLOSION
    }

    public Minesweeper(int width, int height, int bombCount)
    {
        _grid = new MinesweeperGrid(width, height, bombCount);
        _observers = new List<IObserver>();
        _updatedCells = new List<CellUpdate>();
    }

    public List<CellUpdate> GetUpdatedCells()
    {
        return _updatedCells;
    }

    public void SwitchMarkCell(int x, int y)
    {
        if (!_grid.IsTurned(x, y))
        {
            _grid.SwitchMarkCell(x, y);
            CellUpdateType type = (_grid.IsMarked(x, y)) ? CellUpdateType.MARKED : CellUpdateType.UNMARKED;
            _updatedCells.Add(new CellUpdate(type, x, y, -1));
            NotifyObservers();
        }
    }

    public void DiscoverFirstCell(int x, int y)
    {
        List<Cell> bombList;

        // [2]
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
            _updatedCells.Add(new CellUpdate(CellUpdateType.DISCOVERED, x, y, adjacentBombCount));

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
            _updatedCells.Add(new CellUpdate(CellUpdateType.EXPLOSION, x, y, -1));
        }

        if (level == 0) NotifyObservers();
    }

    // IObservable

    public void RegisterObserver(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void NotifyObservers()
    {
        foreach (IObserver o in _observers)
        {
            o.UpdateFromObservable();
        }

        _updatedCells.Clear();
    }
}
