
using System.Collections.Generic;




namespace Mesint_RollingCube_console
{
    public class DFSSolver : ISolver
    {
        public List<CubeState> Solve(BoardState start, int maxSteps)
            => Solve(start);

        public List<CubeState> Solve(BoardState start)
        {
            var stack = new Stack<SearchNode>();
            var visited = new HashSet<BoardState>();

            stack.Push(new SearchNode(start, null, null, 0, 0));

            while (stack.Count > 0)
            {
                var node = stack.Pop();

                if (node.State.IsGoal())
                    return ReconstructPath(node);

                if (!visited.Add(node.State))
                    continue;

                foreach (var move in node.State.GetValidMoves())
                {
                    var nextState = node.State.Apply(move);
                    var child = new SearchNode(nextState, node, move, node.G + 1, 0);
                    stack.Push(child);
                }
            }

            return null;
        }

        private List<CubeState> ReconstructPath(SearchNode node)
        {
            var path = new List<CubeState>();
            while (node != null)
            {
                path.Add(node.State.Cube);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }
    }
}

