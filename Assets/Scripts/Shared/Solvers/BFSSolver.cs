
using System.Collections.Generic;




namespace Mesint_RollingCube_console
{
    public class BFSSolver : ISolver
    {
        public List<CubeState> Solve(BoardState start, int maxSteps)
            => Solve(start);

        public List<CubeState> Solve(BoardState start)
        {
            var queue = new Queue<SearchNode>();
            var visited = new HashSet<BoardState>();

            var root = new SearchNode(start, null, null, 0, 0);
            queue.Enqueue(root);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                if (node.State.IsGoal())
                    return ReconstructPath(node);

                foreach (var move in node.State.GetValidMoves())
                {
                    var nextState = node.State.Apply(move);
                    if (visited.Add(nextState))
                    {
                        var child = new SearchNode(nextState, node, move, node.G + 1, 0);
                        queue.Enqueue(child);
                    }
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