using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesint_RollingCube_console
{
    public class RandomSolver
    {
        private readonly Random rnd = new Random();

        public BoardState Solve(BoardState start, int maxSteps)
        {
            var current = start;
            int steps = 0;
            for (int i = 0; i < maxSteps && !current.IsGoal(); i++)
            {
                var moves = current.GetValidMoves();
                var list = new List<Move>(moves);
                if (list.Count == 0) break;
                var pick = list[rnd.Next(list.Count)];
                current = current.Apply(pick);
                steps++;
            }
            //Console.WriteLine("Lépések száma: "+ steps);
            return current.IsGoal() ? current : null;
        }
    }

    public class BacktrackSolver
    {
        public BoardState Solve(BoardState start, int maxDepth)
            => Search(start, 0, maxDepth, new HashSet<BoardState>());

        private BoardState Search(BoardState state, int depth, int maxDepth, HashSet<BoardState> visited)
        {
            if (state.IsGoal()) return state;
            if (depth >= maxDepth || visited.Contains(state)) return null;

            visited.Add(state);
            foreach (var m in state.GetValidMoves())
            {
                var next = state.Apply(m);
                var result = Search(next, depth + 1, maxDepth, visited);
                if (result != null) return result;
            }
            return null;
        }
    }

    public class DFSSolver
    {
        public BoardState Solve(BoardState start)
        {
            var stack = new Stack<BoardState>();
            var visited = new HashSet<BoardState>();
            stack.Push(start);

            while (stack.Count > 0)
            {
                var node = stack.Pop();
                if (node.IsGoal()) return node;
                visited.Add(node);

                foreach (var m in node.GetValidMoves())
                {
                    var child = node.Apply(m);
                    if (!visited.Contains(child))
                        stack.Push(child);
                }
            }
            return null;
        }
    }
}