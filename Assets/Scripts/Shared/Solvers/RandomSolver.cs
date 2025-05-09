using System;
using System.Collections.Generic;
using System.Linq;


namespace Mesint_RollingCube_console
{
    public class RandomSolver : ISolver
    {
        private readonly Random rnd = new Random();

        public List<CubeState> Solve(BoardState start, int maxSteps)
        {
            var path = new List<CubeState> { start.Cube };
            var current = start;
            int steps = 0;
            for (int i = 0; i < maxSteps && !current.IsGoal(); i++)
            {
                var moves = current.GetValidMoves().ToList();
                if (moves.Count == 0) break;

                var pick = moves[rnd.Next(moves.Count)];
                current = current.Apply(pick);
                path.Add(current.Cube); // minden lépés után mentjük
                steps++;
            }
            //Console.WriteLine("Lépések száma: "+ steps);
            return current.IsGoal() ? path : null;
        }

        public List<CubeState> Solve(BoardState start)
        {
            return Solve(start, 1000);
        }
    }

}