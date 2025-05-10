
using System.Collections.Generic;


namespace Mesint_RollingCube_console
{
    public class BacktrackSolver : ISolver
    {
        private readonly int maxDepth;
        private readonly bool circleCheck;

        public BacktrackSolver(int maxDepth = int.MaxValue, bool circleCheck = true)
        {
            this.maxDepth = maxDepth;
            this.circleCheck = circleCheck;
        }

        public List<CubeState> Solve(BoardState start, int maxSteps)
            => Solve(start);

        public List<CubeState> Solve(BoardState start)
        {
            var root = new SearchNode(start, null, null, 0, 0);
            var goal = Search(root, 0, maxDepth, new HashSet<BoardState>());
            return goal != null ? ReconstructPath(goal) : null;
        }

        private SearchNode Search(SearchNode node, int depth, int maxDepth, HashSet<BoardState> visited)
        {
            if (node.State.IsGoal())
                return node;

            if (depth >= maxDepth || visited.Contains(node.State))
                return null;

            // körfigyelés a láncon
            if (circleCheck)
            {
                for (var anc = node.Parent; anc != null; anc = anc.Parent)
                    if (anc.State.Equals(node.State))
                        return null;
            }

            visited.Add(node.State);

            foreach (var move in node.State.GetValidMoves())
            {
                var nextState = node.State.Apply(move);
                var child = new SearchNode(nextState, node, move, node.G + 1, 0);
                var result = Search(child, depth + 1, maxDepth, visited);
                if (result != null) return result;
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
