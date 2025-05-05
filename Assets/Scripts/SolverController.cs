using UnityEngine;
using System.Collections.Generic;
using Mesint_RollingCube_console;
using System.Linq;
using System.Collections;
using System;
public class SolverController : MonoBehaviour
{
    [SerializeField] private GridSpawner gridSpawner;
    [SerializeField] private CubeMover cubeMover;
    public void HandleSolveRequest()
    {
        if (gridSpawner == null || !gridSpawner.IsReady)
        {
            Debug.LogError("GridSpawner nincs kész vagy nincs beállítva.");
            return;
        }

        StartCoroutine(Solve(gridSpawner.GetBoardState()));
    }

    private IEnumerator Solve(BoardState boardState)
    {
        yield return new WaitForSeconds(1f);

        var solver = new RandomSolver();
        var cubeSteps = solver.Solve(boardState, 1000);

        if (cubeSteps == null)
        {
            Debug.Log("Nincs megoldás.");
            yield break;
        }

        List<(Vector3 pos, int redFace)> steps = new();

        foreach (var cube in cubeSteps)
        {
            Vector3 worldPos = GridToWorld(cube.X, cube.Y);
            int redIndex = Array.FindIndex(cube.Faces, f => f == FaceColor.Red);
            steps.Add((worldPos, redIndex));
        }

        cubeMover.EnqueueMoves(steps);
    }

    private Vector3 GridToWorld(int x, int y)
    {
        //float offset = gridSpawner.GridSize * 0.5f - 0.5f;
        float offset = 0;
        return new Vector3(x - offset, 0f, y - offset);
    }

 

}
