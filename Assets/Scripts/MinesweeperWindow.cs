using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesweeperWindow : MonoBehaviour
{
    public int _width = 5;
    public int _heigth = 5;
    public int _bombCount = 5;

    public GameObject _camera;
    public int _cameraDistance;

    public float _scale = 1f;
    public float _moveAreaSize = 2f;

    private GameObject[,] _objectGrid;
    private GameObject _moveArea;

    private bool _firstMoveDone = false;
    private (int, int) _lastMove = (-1, -1);

    private bool _followCursor = false;
    private Vector3 _cursorOffset = Vector3.zero;

    private Minesweeper _minesweeper;

    void Start()
    {
        _minesweeper = new Minesweeper(_width, _heigth, _bombCount);
        InstantiateObjects();
        CenterCamera();

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(_width * _scale, _heigth * _scale);
        collider.offset = new Vector2(_width * _scale / 2 - _scale / 2, _heigth * _scale / 2 - _scale / 2);
    }

    private void InstantiateObjects()
    {
        _objectGrid = new GameObject[_heigth, _width];

        // Cells
        for (int i = 0; i < _heigth; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                GameObject unclickedPrefab = Instantiate(PrefabFactory.GetUncoveredCellPrefab(), new Vector3(j * _scale, i * _scale, 0), Quaternion.identity, transform);
                unclickedPrefab.GetComponent<SpriteRenderer>().size = new Vector3(_scale, _scale, 1);
                _objectGrid[i, j] = unclickedPrefab;
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

            List<(int, int, int)> discoveredCells = _firstMoveDone ? _minesweeper.DiscoverCell(x, y) : _minesweeper.DiscoverFirstCell(x, y);
            if (!_firstMoveDone)
            {
                _firstMoveDone = true;
            }

            for (int i = 0; i < discoveredCells.Count; i++)
            {
                (int, int, int) cell = discoveredCells[i];
                DiscoverCell(cell.Item1, cell.Item2, cell.Item3);
            }
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

        GameObject prefab = (bombs == 9) ? PrefabFactory.GetBombPrefab() : PrefabFactory.GetDiscoveredCellPrefab(bombs);
        GameObject clickedPrefab = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
        
        clickedPrefab.transform.localPosition = new Vector3(x * _scale, y * _scale, 0);
        clickedPrefab.GetComponent<SpriteRenderer>().size = new Vector3(_scale, _scale, 1);
        _objectGrid[y, x] = clickedPrefab;
    }

    private void ReplaceCell(int x, int y, GameObject newPrefab)
    {
        Destroy(_objectGrid[y, x]);

        GameObject cellPrefab = Instantiate(newPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);

        cellPrefab.transform.localPosition = new Vector3(x * _scale, y * _scale, 0);
        cellPrefab.GetComponent<SpriteRenderer>().size = new Vector3(_scale, _scale, 1);
        _objectGrid[y, x] = cellPrefab;
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
            _moveArea = Instantiate(PrefabFactory.GetRectPrefab(), new Vector3(transform.position.x + _width * _scale / 2 - _scale / 2, transform.position.y + _heigth * _scale / 2 - _scale / 2, 1), Quaternion.identity, transform);
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
}
