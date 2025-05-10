using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mesint_RollingCube_console;
using UnityEngine;
public class SolverController : MonoBehaviour
{
    [Header("Hivatkozások")]
    [SerializeField] private GridSpawner gridSpawner;
    [SerializeField] private CubeMover cubeMover;

    [Header("Megoldási beállítások")]
    [SerializeField] private SolverType solverType = SolverType.TrialError;

    [SerializeField] private bool useMaxStepLimit = true;
    [SerializeField] private int maxSteps = 1000;
    [SerializeField] private bool circleCheck = false;

    [SerializeField] private bool allowRestart = false;
    [SerializeField] private int maxRestartAttempts = 3;
    [SerializeField] private int maxDepth = 20;


    [Header("Events")]
    [SerializeField] private SolverStartedEvent solverStartedEvent;

    [SerializeField] private MessageEvent MessageEvent;
    [SerializeField] private GridChangedEvent gridChangedEvent;
    [SerializeField] private string warningMessage = "";

    public event Action OnSolveCompleted;

    private BoardState boardStateSaving = null;

    void OnEnable()
    {
        gridChangedEvent.OnEvent += OnGridChangedEvent;
    }
    void OnDisable()
    {
        gridChangedEvent.OnEvent -= OnGridChangedEvent;
    }

    public void HandleSolveRequest()
    {
        if (gridSpawner == null || !gridSpawner.IsReady)
        {
            warningMessage = "GridSpawner nincs kész vagy nincs beállítva.";
            MessageEvent.Raise(warningMessage);
            Debug.LogError(warningMessage);
            return;
        }
        solverStartedEvent?.Raise(solverType.ToString());


        if (boardStateSaving == null)
            StartCoroutine(Solve(gridSpawner.GetBoardState()));
        else
            StartCoroutine(Solve(boardStateSaving));
    }

    public void OnGridChangedEvent()
    {
        boardStateSaving = null;
    }

    private IEnumerator Solve(BoardState boardState)
    {
        yield return new WaitForSeconds(1f);

        /*  var solver = new RandomSolver();
          var cubeSteps = solver.Solve(boardState, 1000);

          if (cubeSteps == null)
          {
              Debug.Log("Nincs megoldás.");
              yield break;
          }
  */
        boardStateSaving = boardState;
        List<CubeState> solution = null;
        int attempts = 0;

        do
        {
            ISolver solver = CreateSolver(solverType, circleCheck);
            solution = useMaxStepLimit
                ? solver.Solve(boardState, maxSteps)
                : solver.Solve(boardState);

            if (solution != null)
            {
                warningMessage = $"Megoldás {attempts + 1}. próbálkozásra megtalálva.";
                MessageEvent.Raise(warningMessage);
                Debug.Log(warningMessage);
                break;
            }

            attempts++;

        } while (allowRestart && attempts < maxRestartAttempts);

        if (solution == null)
        {
            warningMessage = "Nem sikerült megoldást találni a megadott próbálkozások alatt.";
            MessageEvent.Raise(warningMessage);
            Debug.LogWarning(warningMessage);
            OnSolveCompleted?.Invoke();
            yield break;
        }

        List<(Vector3 pos, int redFace)> steps = new();

        foreach (var cube in solution)
        {
            Vector3 worldPos = GridToWorld(cube.X, cube.Y);
            int redIndex = Array.FindIndex(cube.Faces, f => f == FaceColor.Red);
            steps.Add((worldPos, redIndex));
        }

        cubeMover.EnqueueMoves(steps);

        //OnSolveCompleted?.Invoke();
    }

    private ISolver CreateSolver(SolverType type, bool circleCheck)
    {
        return type switch
        {
            SolverType.TrialError => new RandomSolver(),
            SolverType.DFS => new DFSSolver(circleCheck),
            SolverType.Backtrack => new BacktrackSolver(maxDepth, circleCheck),
            SolverType.BFS => new BFSSolver(circleCheck),
            SolverType.AStar => new AStarSolver(circleCheck),
            _ => new RandomSolver()
        };
    }

    private Vector3 GridToWorld(int x, int y)
    {
        //float offset = gridSpawner.GridSize * 0.5f - 0.5f;
        float offset = 0;
        return new Vector3(x - offset, 0f, y - offset);
    }



}
