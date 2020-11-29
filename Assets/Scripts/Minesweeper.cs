using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minesweeper : MonoBehaviour
{
    public GameObject _unclickedPrefab;
    public GameObject _bombHintPrefab;
    public GameObject _moveAreaPrefab;

    public GameObject _camera;
    public int _cameraDistance;

    public int _width = 5;
    public int _heigth = 5;
    public int _bombCount = 5;

    private bool[,] _grid;
    private List<GameObject> _objectGrid;

    // Start is called before the first frame update
    void Start()
    {
        _grid = GenerateGrid();
        InstantiateObjects(_grid);
        CenterCamera();
    }

    private void InstantiateObjects(bool[,] grid)
    {
        _objectGrid = new List<GameObject>();

        // Move Area
        GameObject moveArea = Instantiate(_moveAreaPrefab, new Vector3(grid.GetLength(1) / 2, grid.GetLength(0), 0), Quaternion.identity, transform);
        moveArea.transform.localScale = new Vector3(grid.GetLength(1), 3.1f, 1);
        _objectGrid.Add(moveArea);

        // Cells
        for (int i = 0; i < grid.GetLength(0); i++) {
            for (int j = 0; j < grid.GetLength(1); j++) {
                if (grid[i, j]) {
                    _objectGrid.Add(Instantiate(_bombHintPrefab, new Vector3(j, i, 0), Quaternion.identity, transform));
                }
                else {
                    _objectGrid.Add(Instantiate(_unclickedPrefab, new Vector3(j, i, 0), Quaternion.identity, transform));
                }
            }
        }
    }

    private bool[,] GenerateGrid() {
        bool[,] grid = new bool[_heigth,_width];
        
        for (int i = 0; i < grid.GetLength(0); i++) {
            for (int j = 0; j < grid.GetLength(1); j++) {
                grid[i, j] = false;
            }
        }

        for (int i = 0; i < _bombCount; i++) {
            int x, y;
            do
            {
                x = Mathf.FloorToInt(Random.value * _width);
                y = Mathf.FloorToInt(Random.value * _heigth);
            } while (grid[y,x]);
            grid[y,x] = true;
            Debug.Log("Bomb at (" + x + ";" + y + ")");
        }

        return grid;
    }

    private void CenterCamera()
    {
        _camera.transform.position = new Vector3(_grid.GetLength(1) / 2, _grid.GetLength(0) / 2, -_cameraDistance);
        _camera.GetComponent<Camera>().orthographicSize = _cameraDistance;
    }
}
