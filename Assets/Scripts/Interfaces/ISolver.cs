using System.Collections.Generic;
using Mesint_RollingCube_console;


public interface ISolver
{
    List<CubeState> Solve(BoardState start, int maxSteps); // ha van max lépés
    List<CubeState> Solve(BoardState start);               // ha nincs
}

