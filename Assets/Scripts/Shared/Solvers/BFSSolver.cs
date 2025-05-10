
using System.Collections.Generic;




namespace Mesint_RollingCube_console
{
    public class BFSSolver : ISolver
    {
        private readonly bool circleCheck;

        public BFSSolver(bool circleCheck = true)
        {
            this.circleCheck = circleCheck;
        }

        public List<CubeState> Solve(BoardState start, int maxSteps)
            => Solve(start);

        public List<CubeState> Solve(BoardState start)
        {
            var queue = new Queue<SearchNode>();
            var visited = new HashSet<BoardState>();

            queue.Enqueue(new SearchNode(start, null, null, 0, 0));
            if (circleCheck)
                visited.Add(start);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                if (node.State.IsGoal())
                    return ReconstructPath(node);

                foreach (var move in node.State.GetValidMoves())
                {
                    var next = node.State.Apply(move);

                    // körfigyelés
                    if (circleCheck)
                    {
                        if (visited.Contains(next)) continue;
                        visited.Add(next);
                    }

                    // ha nincs circleCheck, minden ágat beengedünk (vigyázat: végtelen is lehet)
                    var child = new SearchNode(next, node, move, node.G + 1, 0);
                    queue.Enqueue(child);
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