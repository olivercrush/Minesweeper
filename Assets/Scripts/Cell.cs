using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>Cell</c> models a cell in a 2D Minesweeper grid
/// </summary>
public class Cell
{
    private int _x;
    private int _y;

    private bool _isBomb;
    private bool _isTurned;
    private bool _isMarked;
    
    public Cell(int x, int y, bool isBomb)
    {
        _x = x;
        _y = y;
        _isBomb = isBomb;
        _isTurned = false;
        _isMarked = false;
    }

    public int GetDistance(int x, int y)
    {
        return Mathf.Abs(_x - x) + Mathf.Abs(_y - y);
    }

    // GETTERS

    public bool IsBomb()
    {
        return _isBomb;
    }

    public bool IsTurned()
    {
        return _isTurned;
    }

    public bool IsMarked()
    {
        return _isMarked;
    }

    // SETTERS

    public (int, int) GetCoordinates()
    {
        return (_x, _y);
    }

    public void SetBomb(bool isBomb)
    {
        _isBomb = isBomb;
    }

    public void TurnCell()
    {
        _isTurned = true;
    }

    public void SwitchMarked()
    {
        _isMarked = !_isMarked;
    }
}
