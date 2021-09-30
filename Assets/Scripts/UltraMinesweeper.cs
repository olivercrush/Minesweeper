using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltraMinesweeper : MonoBehaviour
{
    public GameObject _camera;
    public GameObject _gameOverUi;
    public GameObject _gameWonUi;

    void Start() {
        GameObject minesweeper = PrefabFactory.GetMinesweeperPrefab(10, 5, 15, _camera);
        GameObject.Instantiate(minesweeper);
    }
}
