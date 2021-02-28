using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesweeperWindow : MonoBehaviour, IObserver
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
        _minesweeper.RegisterObserver(this);

        InstantiateObjects();
        CenterCamera();

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(_width * _scale, _heigth * _scale);
        collider.offset = new Vector2(_width * _scale / 2 - _scale / 2, _heigth * _scale / 2 - _scale / 2);
    }

    private void Update()
    {
        DiscoverAction();
        MarkAction();
    }

    private void FixedUpdate()
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

    private void DiscoverAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0.3f;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            if (IsMouseOnMinesweeper(mousePos))
            {
                int x = Mathf.CeilToInt(mousePos.x - transform.position.x - _scale / 2);
                int y = Mathf.CeilToInt(mousePos.y - transform.position.y - _scale / 2);

                if (_firstMoveDone)
                {
                    _minesweeper.DiscoverCell(x, y);
                }
                else
                {
                    _minesweeper.DiscoverFirstCell(x, y);
                    _firstMoveDone = true;
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
    }

    private void MarkAction()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0.3f;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            if (IsMouseOnMinesweeper(mousePos))
            {
                int x = Mathf.CeilToInt(mousePos.x - transform.position.x - _scale / 2);
                int y = Mathf.CeilToInt(mousePos.y - transform.position.y - _scale / 2);
                _minesweeper.SwitchMarkCell(x, y);
            }
        }
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

    // IObserver

    public void UpdateFromObservable()
    {
        foreach (Minesweeper.CellUpdate c in _minesweeper.GetUpdatedCells())
        {
            switch(c.type)
            {
                case Minesweeper.CellUpdateType.DISCOVERED:
                    ReplaceCell(c.x, c.y, PrefabFactory.GetDiscoveredCellPrefab(c.moore_bombs_count));
                    break;

                case Minesweeper.CellUpdateType.EXPLOSION:
                    ReplaceCell(c.x, c.y, PrefabFactory.GetBombPrefab());
                    break;

                case Minesweeper.CellUpdateType.MARKED:
                    ReplaceCell(c.x, c.y, PrefabFactory.GetMarkedPrefab());
                    break;

                case Minesweeper.CellUpdateType.UNMARKED:
                    ReplaceCell(c.x, c.y, PrefabFactory.GetUncoveredCellPrefab());
                    break;
            }
        }
    }

    private void ReplaceCell(int x, int y, GameObject newPrefab)
    {
        Destroy(_objectGrid[y, x]);

        GameObject cellPrefab = Instantiate(newPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);

        cellPrefab.transform.localPosition = new Vector3(x * _scale, y * _scale, 0);
        cellPrefab.GetComponent<SpriteRenderer>().size = new Vector3(_scale, _scale, 1);
        _objectGrid[y, x] = cellPrefab;
    }

    private void InstantiateObjects()
    {
        _objectGrid = new GameObject[_heigth, _width];

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
}
