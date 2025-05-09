using UnityEngine;
using System.Collections.Generic;
using Mesint_RollingCube_console;
using System.Linq;
using System.Collections;


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

    [SerializeField]
    private bool[,] obstacleGrid;
    private CubeState startCube;
    [SerializeField]
    private (int X, int Y) goalCell;

    private FaceColor[] initialFaces;

    // belső állapot: már foglalt mezők
    private readonly HashSet<Vector2Int> occupied = new();

    [Header("Events")]
    [SerializeField]
    private GridChangedEvent gridChangedEvent;

  void Awake()
    {
        ClearGrid();
        occupied.Clear();
       /* if (Application.isPlaying)
        {

            Initialize();
            Debug.Log("Intial");
        }
        */
    }

    void Start()
    {

    }

    void OnEnable()
    {
        /*
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Spawn(grayCubePrefab, obstacleCount);
            Spawn(redCubePrefab);
            Spawn(goalPrefab);

        }
#endif
*/


    }

    void OnDisable()
    {
        // amikor kikapcsolod a komponens(és az EditMode-ban is),
        // töröljük a korábbi child-okat, és ürítjük a pozíciólistát
        ClearGrid();
        occupied.Clear();

    }

    void OnDestroy()
    {
        ClearGrid();
        occupied.Clear();
    }
    public void Initialize()
    {
        ClearGrid();
        occupied.Clear();

        Spawn(grayCubePrefab, obstacleCount);
        Spawn(redCubePrefab);
        Spawn(goalPrefab);

        StartCoroutine(BuildStateRepresentation());

        gridChangedEvent.Raise();
       // StartCoroutine(DelayedSolve());

    }

    IEnumerator DelayedSolve()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("Sorrend");
        var solver = new RandomSolver();
        var result = solver.Solve(BuildBoardState(), 1000);
        if (result != null)
            Debug.Log("Megoldás találva!");
        else
            Debug.Log("Nincs megoldás random módszerrel.");

        yield return null;
    }

    private void PrintOccupied()
    {
        if (occupied.Count == 0)
        {
            Debug.Log("Occupied mezők: (üres)");
            return;
        }

        string output = "Occupied mezők:";
        foreach (var cell in occupied)
        {
            output += $" ({cell.x},{cell.y})";
        }

        Debug.Log(output);
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

    private void PrintObstacleGrid()
    {
        if (obstacleGrid == null)
        {
            Debug.LogWarning("obstacleGrid még nincs inicializálva.");
            return;
        }

        string output = "Obstacle Grid:\n";
        for (int y = gridSize - 1; y >= 0; y--) // fentről lefelé olvasható
        {
            for (int x = 0; x < gridSize; x++)
            {
                output += obstacleGrid[x, y] ? "X " : ". ";
            }
            output += "\n";
        }

        Debug.Log(output);
    }

    IEnumerator BuildStateRepresentation()
    {
        Debug.Log("Build");
        PrintOccupied();
        PrintObstacleGrid();
        // 1) Obstacle-ok leképezése gridbe
        obstacleGrid = new bool[gridSize, gridSize];
        foreach (Transform c in gridParent)
        {

            Vector2Int cell = WorldToGrid(c.position);
            if (c.parent != gridParent)
                continue;


            switch (c.tag)
            {

                case "Obstacle":
                    obstacleGrid[cell.x, cell.y] = true;
                    break;

                case "Red":
                    // Itt határozzuk meg initialFaces-t
                    initialFaces = DetermineInitialFaces(c);
                    startCube = new CubeState(cell.x, cell.y, initialFaces);
                    break;

                case "Goal":
                    goalCell = (cell.x, cell.y);
                    Debug.Log("goal cell: " + goalCell.X + ":" + goalCell.Y);
                    break;
            }
        }
        Debug.Log("Build vége");
        PrintObstacleGrid();
        yield return null;

    }

    FaceColor[] DetermineInitialFaces(Transform redCube)
    {
        // Alap: minden oldal kék
        var faces = Enumerable.Repeat(FaceColor.Blue, 6).ToArray();

        // Keresd meg a child quad-ot, amin a piros színt rajtad van
        var redSide = redCube.GetComponentsInChildren<Transform>()
                             .FirstOrDefault(t => t.CompareTag("Red_side"));
        if (redSide == null)
        {
            Debug.LogError("Nem találom a Red_side quadot a Red kocka alatt!");
            return faces;
        }

        // A quad normálvektorét vesszük iránynak: 
        // ha a quad Plane meshénél az +Z a "normál", akkor
        Vector3 localNormal = redSide.localRotation * Vector3.forward;

        // Most megnézzük, hogy melyik fő irányhoz áll közel:
        float maxDot = -1f;
        int faceIndex = -1;
        Vector3[] directions = {
            Vector3.up,    // 0 = Top
            Vector3.down,  // 1 = Bottom
            Vector3.forward,  // 2 = Front (előre)
            Vector3.back,     // 3 = Back
            Vector3.left,     // 4 = Left
            Vector3.right     // 5 = Right
        };

        for (int i = 0; i < directions.Length; i++)
        {
            float d = Vector3.Dot(localNormal, directions[i]);
            if (d > maxDot)
            {
                maxDot = d;
                faceIndex = i;
            }
        }

        if (faceIndex >= 0)
            faces[faceIndex] = FaceColor.Red;

        return faces;
    }
    BoardState BuildBoardState()
        => new BoardState(gridSize, obstacleGrid, startCube, goalCell);


    public BoardState GetBoardState()
    {
        return BuildBoardState();
    }

    public bool IsReady => startCube != null && obstacleGrid != null;

    private Vector3 GridToWorld(Vector2Int grid)
    {
        float offset = 0;
        return new Vector3(grid.x - offset, 0f, grid.y - offset);
    }


    Vector2Int WorldToGrid(Vector3 w)
    {


        int x = Mathf.RoundToInt(w.x);
        int y = Mathf.RoundToInt(w.z);
        return new Vector2Int(x, y);
    }
}
