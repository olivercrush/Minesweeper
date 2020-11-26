using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minesweeper : MonoBehaviour
{
    public GameObject _coveredCellPrefab;
    public GameObject _coveredBombPrefab;

    private bool[,] _grid;
    private List<GameObject> _objectGrid;

    // Start is called before the first frame update
    void Start()
    {
        _grid = GenerateGrid();
        InstantiateObjects(_grid);
    }

    private void InstantiateObjects(bool[,] grid)
    {
        _objectGrid = new List<GameObject>();
        for (int i = 0; i < grid.GetLength(0); i++) {
            for (int j = 0; j < grid.GetLength(1); j++) {
                if (grid[i, j]) {
                    _objectGrid.Add(Instantiate(_coveredBombPrefab, new Vector3(j, i, 0), Quaternion.identity, transform));
                }
                else {
                    _objectGrid.Add(Instantiate(_coveredCellPrefab, new Vector3(j, i, 0), Quaternion.identity, transform));
                }
            }
        }
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
