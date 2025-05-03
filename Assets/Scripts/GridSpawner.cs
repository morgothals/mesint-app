using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class GridSpawner : MonoBehaviour
{
    [Header("Grid Settings")]
    public Transform gridParent;
    public int gridSize = 10;
    public int obstacleCount = 10;

    [Header("Prefabs")]
    public GameObject grayCubePrefab;
    public GameObject redCubePrefab;
    public GameObject goalPrefab;

    // belső állapot: már foglalt mezők
    private readonly HashSet<Vector2Int> occupied = new();

    void Start()
    {
        if (Application.isPlaying)
            Initialize();
    }

    void OnEnable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Spawn(grayCubePrefab, obstacleCount);
            Spawn(redCubePrefab);
            Spawn(goalPrefab);

        }
#endif
    }

    private void Initialize()
    {
        ClearGrid();
        occupied.Clear();

        Spawn(grayCubePrefab, obstacleCount);
        Spawn(redCubePrefab);
        Spawn(goalPrefab);
    }

    private void ClearGrid()
    {
        // hátulról elölre, így biztos nem hagyunk ki semmit
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            var go = gridParent.GetChild(i).gameObject;
            if (Application.isPlaying)
                Destroy(go);
            else
                DestroyImmediate(go);
        }
    }

    void OnDisable()
    {
        // amikor kikapcsolod a komponens(és az EditMode-ban is),
        // töröljük a korábbi child-okat, és ürítjük a pozíciólistát
        ClearGrid();
        occupied.Clear();
    }

    /// <summary>
    /// Véletlenszerűen spawnol <paramref name="count"/> darabot a megadott prefab-ból,
    /// úgy, hogy ne ütközzenek egymással.
    /// </summary>
    private void Spawn(GameObject prefab, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2Int gridPos = PickFreePosition();
            occupied.Add(gridPos);

            Vector3 worldPos = GridToWorld(gridPos);
            Instantiate(prefab, worldPos, prefab.transform.rotation, gridParent);
        }
    }

    /// <summary>
    /// Visszaad egy olyan véletlen cellát, ami még szabad.
    /// </summary>
    private Vector2Int PickFreePosition()
    {
        Vector2Int pos;
        do
        {
            pos = new Vector2Int(
                Random.Range(0, gridSize),
                Random.Range(0, gridSize)
            );
        } while (occupied.Contains(pos));

        return pos;
    }

    /// <summary>
    /// Grid-koordinátát világkoordinátává alakít,
    /// hogy a kirajzott tábla középre essen.
    /// </summary>
    private Vector3 GridToWorld2(Vector2Int grid)
    {
        float offset = (gridSize) * 0.5f - 1;
        return new Vector3(grid.x - offset, 0f, grid.y - offset);
    }

    private Vector3 GridToWorld(Vector2Int grid)
    {
        float offset = (gridSize - 1) * 0.5f;
        return new Vector3(grid.x - offset, 0f, grid.y - offset);
    }
}
