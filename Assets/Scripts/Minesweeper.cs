using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minesweeper : MonoBehaviour
{
    public GameObject _unclickedPrefab;
    public GameObject _bombHintPrefab;
    public GameObject _rectPrefab;

    public GameObject _camera;
    public int _cameraDistance;

    public int _width = 5;
    public int _heigth = 5;
    public int _bombCount = 5;

    public float _scale = 1f;
    public float _moveAreaSize = 2f;

    private bool[,] _grid;
    private List<GameObject> _objectGrid;
    private GameObject _moveArea;

    private bool _followCursor = false;
    private Vector3 _cursorOffset = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        _grid = GenerateGrid();
        InstantiateObjects(_grid);
        CenterCamera();

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(_width * _scale, _heigth * _scale);
        collider.offset = new Vector2(_width * _scale / 2 - _scale / 2, _heigth * _scale / 2 - _scale / 2);

        // Vector2 spriteSize = _unclickedPrefab.GetComponent<SpriteRenderer>().size * _scale;
    }

    private void Update()
    {
        if (_followCursor && _cursorOffset != Vector3.zero)
        {
            transform.position = Input.mousePosition + _cursorOffset;
        }
    }

    private void OnMouseDown()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0.3f;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        if (IsMouseOnMinesweeper(mousePos))
        {
            Debug.Log("in");
        }
        else
        {
            Debug.Log("out");
        }
    }

    private void OnMouseUp()
    {
        
    }

    private void OnMouseEnter()
    {
        GetComponent<BoxCollider2D>().size = new Vector2(_width * _scale + _moveAreaSize, _heigth * _scale + _moveAreaSize);
        _moveArea = Instantiate(_rectPrefab, new Vector3(_width * _scale / 2 - _scale / 2, _heigth * _scale / 2 - _scale / 2, 1), Quaternion.identity, transform);
        _moveArea.GetComponent<SpriteRenderer>().size = new Vector2(_width * _scale + _moveAreaSize, _heigth * _scale + _moveAreaSize);
        _moveArea.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
    }

    private void OnMouseExit()
    {
        Destroy(_moveArea);
        _moveArea = null;
        GetComponent<BoxCollider2D>().size = new Vector2(_width * _scale, _heigth * _scale);
    }

    public void StartMinesweeperMove(Vector3 cursorOffset)
    {
        _cursorOffset = cursorOffset;
        _followCursor = true;
    }

    public void StopMinesweeperMove()
    {
        _cursorOffset = Vector3.zero;
        _followCursor = false;
    }

    private void InstantiateObjects(bool[,] grid)
    {
        _objectGrid = new List<GameObject>();

        // Cells
        for (int i = 0; i < grid.GetLength(0); i++) {
            for (int j = 0; j < grid.GetLength(1); j++) {
                if (grid[i, j]) {
                    GameObject bombHint = Instantiate(_bombHintPrefab, new Vector3(j * _scale, i * _scale, 0), Quaternion.identity, transform);
                    bombHint.GetComponent<SpriteRenderer>().size = new Vector3(_scale, _scale, 1);
                    _objectGrid.Add(bombHint);
                }
                else {
                    GameObject unclickedPrefab = Instantiate(_unclickedPrefab, new Vector3(j * _scale, i * _scale, 0), Quaternion.identity, transform);
                    unclickedPrefab.GetComponent<SpriteRenderer>().size = new Vector3(_scale, _scale, 1);
                    _objectGrid.Add(unclickedPrefab);
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
            //Debug.Log("Bomb at (" + x + ";" + y + ")");
        }

        return grid;
    }

    private void CenterCamera()
    {
        _camera.transform.position = new Vector3(_grid.GetLength(1) / 2, _grid.GetLength(0) / 2, -_cameraDistance);
        _camera.GetComponent<Camera>().orthographicSize = _cameraDistance;
    }

    private bool IsMouseOnMinesweeper(Vector3 mousePos)
    {
        return
            mousePos.x > transform.position.x &&
            mousePos.x < transform.position.x + _width * _scale - _scale / 2 &&
            mousePos.y > transform.position.y &&
            mousePos.y < transform.position.y + _heigth * _scale - _scale / 2;    
    }
}
