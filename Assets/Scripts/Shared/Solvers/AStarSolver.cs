using System;
using System.Collections.Generic;
using System.Linq;




namespace Mesint_RollingCube_console
{




    public class AStarSolver : ISolver
    {
        private readonly bool circleCheck;

        public AStarSolver(bool circleCheck = true)
        {
            this.circleCheck = circleCheck;
        }

        public List<CubeState> Solve(BoardState start, int maxSteps)
            => Solve(start);

        public List<CubeState> Solve(BoardState start)
        {
            var open = new List<SearchNode>();
            var closed = new HashSet<BoardState>();

            open.Add(new SearchNode(start, null, null, 0, Heuristic(start)));

            while (open.Count > 0)
            {
                // O(n) keresés a legkisebb F-ű csomópontra
                int best = open.Select((n, i) => (n.F, i))
                               .OrderBy(x => x.F)
                               .First().i;

                var node = open[best];
                open.RemoveAt(best);

                if (node.State.IsGoal())
                    return ReconstructPath(node);

                if (circleCheck)
                    closed.Add(node.State);

                foreach (var move in node.State.GetValidMoves())
                {
                    var nextState = node.State.Apply(move);

                    // ha circleCheck, és már zárt, akkor kihagyjuk
                    if (circleCheck && closed.Contains(nextState))
                        continue;

                    var gNew = node.G + 1;
                    var hNew = Heuristic(nextState);
                    var child = new SearchNode(nextState, node, move, gNew, hNew);

                    // ha circleCheck, nézzük az open listát is
                    if (circleCheck)
                    {
                        var exist = open.FirstOrDefault(n => n.State.Equals(nextState));
                        if (exist != null && exist.G <= gNew)
                            continue;
                        open.Remove(exist);
                    }

                    open.Add(child);
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

        private int Heuristic(BoardState s)
        {
            // Manhattan-távolság a célhoz:
            int dx = Math.Abs(s.Cube.X - s.Goal.X);
            int dy = Math.Abs(s.Cube.Y - s.Goal.Y);
            return dx + dy;
        }
    }

}