using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesweeperWindow : MonoBehaviour
{
    public GameObject _unclickedPrefab;
    public GameObject _ZeroPrefab;
    public GameObject _OnePrefab;
    public GameObject _TwoPrefab;
    public GameObject _ThreePrefab;
    public GameObject _FourPrefab;
    public GameObject _FivePrefab;
    public GameObject _SixPrefab;
    public GameObject _SevenPrefab;
    public GameObject _EightPrefab;

    public GameObject _bombHintPrefab;
    public GameObject _rectPrefab;

    public int _width = 5;
    public int _heigth = 5;
    public int _bombCount = 5;

    public GameObject _camera;
    public int _cameraDistance;

    public float _scale = 1f;
    public float _moveAreaSize = 2f;

    private GameObject[,] _objectGrid;
    private GameObject _moveArea;

    private bool _followCursor = false;
    private Vector3 _cursorOffset = Vector3.zero;

    private Minesweeper _minesweeper;

    void Start()
    {
        _minesweeper = new Minesweeper(_width, _heigth, _bombCount);
        InstantiateObjects(_minesweeper.GetGrid());
        CenterCamera();

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(_width * _scale, _heigth * _scale);
        collider.offset = new Vector2(_width * _scale / 2 - _scale / 2, _heigth * _scale / 2 - _scale / 2);
    }

    private void InstantiateObjects(bool[,] grid)
    {
        _objectGrid = new GameObject[_heigth, _width];

        // Cells
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j])
                {
                    GameObject bombHint = Instantiate(_bombHintPrefab, new Vector3(j * _scale, i * _scale, 0), Quaternion.identity, transform);
                    bombHint.GetComponent<SpriteRenderer>().size = new Vector3(_scale, _scale, 1);
                    _objectGrid[i, j] = bombHint;
                }
                else
                {
                    GameObject unclickedPrefab = Instantiate(_unclickedPrefab, new Vector3(j * _scale, i * _scale, 0), Quaternion.identity, transform);
                    unclickedPrefab.GetComponent<SpriteRenderer>().size = new Vector3(_scale, _scale, 1);
                    _objectGrid[i, j] = unclickedPrefab;
                }
            }
        }
    }

    private void Update()
    {
        if (_followCursor && _cursorOffset != Vector3.zero)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0.3f;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            // Pos : Debug.Log(mousePos - _cursorOffset);
            transform.position = mousePos - _cursorOffset;
        }
    }

    private void OnMouseDown()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0.3f;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        if (IsMouseOnMinesweeper(mousePos))
        {
            int x = Mathf.CeilToInt(mousePos.x - transform.position.x - _scale / 2);
            int y = Mathf.CeilToInt(mousePos.y - transform.position.y - _scale / 2);

            List<(int, int, int)> discoveredCells = _minesweeper.DiscoverCell(x, y);
            //Debug.Log(discoveredCells.Count);
            for (int i = 0; i < discoveredCells.Count; i++)
            {
                (int, int, int) cell = discoveredCells[i];
                DiscoverCell(cell.Item1, cell.Item2, cell.Item3);
            }

            // Debug.Log(_minesweeper.DiscoverCell(x, y));
            // Debug.Log("clicked : " + x + "," + y);
        }
        else
        {
            if (!_followCursor)
            {
                _cursorOffset = mousePos - transform.position;
                _followCursor = true;
            }
        }
    }

    private void DiscoverCell(int x, int y, int bombs)
    {
        Destroy(_objectGrid[y, x]);
        GameObject clickedPrefab = Instantiate(GetCellPrefab(bombs), new Vector3(0, 0, 0), Quaternion.identity, transform);
        clickedPrefab.transform.localPosition = new Vector3(x * _scale, y * _scale, 0);
        clickedPrefab.GetComponent<SpriteRenderer>().size = new Vector3(_scale, _scale, 1);
        _objectGrid[y, x] = clickedPrefab;
    }

    private void OnMouseUp()
    {
        if (_followCursor)
        {
            _followCursor = false;
            _cursorOffset = Vector3.zero;
        }
    }

    private void OnMouseEnter()
    {
        GetComponent<BoxCollider2D>().size = new Vector2(_width * _scale + _moveAreaSize, _heigth * _scale + _moveAreaSize);

        if (_moveArea == null)
        {
            _moveArea = Instantiate(_rectPrefab, new Vector3(transform.position.x + _width * _scale / 2 - _scale / 2, transform.position.y + _heigth * _scale / 2 - _scale / 2, 1), Quaternion.identity, transform);
        }

        _moveArea.GetComponent<SpriteRenderer>().size = new Vector2(_width * _scale + _moveAreaSize, _heigth * _scale + _moveAreaSize);
        _moveArea.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
    }

    private void OnMouseExit()
    {
        // Debug.Log("exited, _followCursor : " + _followCursor);
        CleanMoveArea();
    }

    private void CenterCamera()
    {
        _camera.transform.position = new Vector3(_width / 2, _heigth / 2, -_cameraDistance);
        _camera.GetComponent<Camera>().orthographicSize = _cameraDistance;
    }

    private bool IsMouseOnMinesweeper(Vector3 mousePos)
    {
        return
            mousePos.x > transform.position.x - _scale / 2 &&
            mousePos.x < transform.position.x + _width * _scale - _scale / 2 &&
            mousePos.y > transform.position.y - _scale / 2 &&
            mousePos.y < transform.position.y + _heigth * _scale - _scale / 2;
    }

    private void CleanMoveArea()
    {
        if (!_followCursor)
        {
            Destroy(_moveArea);
            _moveArea = null;
            GetComponent<BoxCollider2D>().size = new Vector2(_width * _scale, _heigth * _scale);
        }
    }

    private GameObject GetCellPrefab(int bombs)
    {
        switch (bombs)
        {
            case 0:
                return _ZeroPrefab;

            case 1:
                return _OnePrefab;

            case 2:
                return _TwoPrefab;

            case 3:
                return _ThreePrefab;

            case 4:
                return _FourPrefab;

            case 5:
                return _FivePrefab;

            case 6:
                return _SixPrefab;

            case 7:
                return _SevenPrefab;

            case 8:
                return _EightPrefab;

            default:
                return _ZeroPrefab;
        }
    }
}
