
using System.Collections.Generic;




namespace Mesint_RollingCube_console
{
    public class DFSSolver : ISolver
    {
        private readonly bool circleCheck;

        public DFSSolver(bool circleCheck = true)
        {
            this.circleCheck = circleCheck;
        }

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

                // ha van circleCheck, használjuk a visited setet is
                if (circleCheck && !visited.Add(node.State))
                    continue;

                foreach (var move in node.State.GetValidMoves())
                {
                    var next = node.State.Apply(move);

                    // ha circleCheck, akkor elkerüljük a parent-ciklust is
                    if (circleCheck)
                    {
                        var p = node;
                        bool loop = false;
                        while (p != null)
                        {
                            if (p.State.Equals(next))
                            {
                                loop = true; break;
                            }
                            p = p.Parent;
                        }
                        if (loop) continue;
                    }

                    var child = new SearchNode(next, node, move, node.G + 1, 0);
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

