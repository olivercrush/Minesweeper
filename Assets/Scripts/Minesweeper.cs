using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minesweeper : MonoBehaviour
{
    private bool[,] _grid;

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    private bool[,] GenerateGrid() {
        bool[,] grid = new bool[5,5];
        
        for (int i = 0; i < grid.GetLength(0); i++) {
            for (int j = 0; j < grid.GetLength(1); j++) {
                grid[i, j] = false;
            }
        }

        for (int i = 0; i < 5; i++) {
            int x, y;
            do
            {
                x = Mathf.FloorToInt(Random.value * 5);
                y = Mathf.FloorToInt(Random.value * 5);
            } while (grid[x,y]);
            grid[x, y] = true;
            Debug.Log("Bomb at (" + x + ";" + y + ")");
        }

        return grid;
    }
}
