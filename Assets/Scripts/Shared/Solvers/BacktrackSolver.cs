
using System.Collections.Generic;


namespace Mesint_RollingCube_console
{
    public class BacktrackSolver : ISolver
    {
        public List<CubeState> Solve(BoardState start, int maxDepth)
        {
            var visited = new HashSet<BoardState>();
            var root = new SearchNode(start, null, null, 0, 0);
            var goalNode = Search(root, 0, maxDepth, visited);
            return goalNode != null ? ReconstructPath(goalNode) : null;
        }

        public List<CubeState> Solve(BoardState start)
            => Solve(start, int.MaxValue);

        private SearchNode Search(SearchNode node, int depth, int maxDepth, HashSet<BoardState> visited)
        {
            if (node.State.IsGoal())
                return node;
            if (depth >= maxDepth || visited.Contains(node.State))
                return null;

            visited.Add(node.State);

            foreach (var move in node.State.GetValidMoves())
            {
                var nextState = node.State.Apply(move);
                var child = new SearchNode(nextState, node, move, node.G + 1, 0);
                var result = Search(child, depth + 1, maxDepth, visited);
                if (result != null)
                    return result;
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
