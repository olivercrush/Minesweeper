using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MinesweeperInfos
{
    public int Width;
    public int Height;
    public int BombCount;
}

public class UltraMinesweeper : MonoBehaviour
{
    [SerializeField]
    public MinesweeperInfos[] _minesweeperInstances;

    public GameObject _camera;
    public GameObject _gameOverUi;
    public GameObject _gameWonUi;

    void Start() {

        int count = 0;
        foreach (MinesweeperInfos m in _minesweeperInstances)
        {
            GameObject instance = PrefabFactory.GetMinesweeperPrefab(m.Width, m.Height, m.BombCount, count, _camera);
            Instantiate(instance);
            count++;
        }
    }
}
