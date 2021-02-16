using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrefabFactory
{
    public static GameObject GetDiscoveredCellPrefab(int bombCount)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Cell_" + bombCount);
        return prefab;
    }

    public static GameObject GetUncoveredCellPrefab()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Cell_Unclicked");
        return prefab;
    }

    public static GameObject GetRectPrefab()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/RectPrefab");
        return prefab;
    }

    public static GameObject GetBombPrefab()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Cell_BombHint");
        return prefab;
    }

    public static GameObject GetLastClickedPrefab()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Cell_LastClicked");
        return prefab;
    }

    public static GameObject GetMarkedPrefab()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Cell_Marked");
        return prefab;
    }
}
