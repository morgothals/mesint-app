using System;
using System.Collections.Generic;
using System.Linq;




namespace Mesint_RollingCube_console
{




    public class AStarSolver : ISolver
    {
        public List<CubeState> Solve(BoardState start, int maxSteps)
            => Solve(start);

        public List<CubeState> Solve(BoardState start)
        {
            var open = new List<SearchNode>();
            var closed = new HashSet<BoardState>();

            // Kezdőcsomópont: G=0, H=heurisztika(start)
            open.Add(new SearchNode(start, null, null, 0, Heuristic(start)));

            while (open.Count > 0)
            {
                // Keresd meg a legkisebb F-ű csomópontot
                int bestIdx = 0;
                for (int i = 1; i < open.Count; i++)
                    if (open[i].F < open[bestIdx].F)
                        bestIdx = i;

                var currentNode = open[bestIdx];
                open.RemoveAt(bestIdx);

                if (currentNode.State.IsGoal())
                    return ReconstructPath(currentNode);

                closed.Add(currentNode.State);

                foreach (var move in currentNode.State.GetValidMoves())
                {
                    var nextState = currentNode.State.Apply(move);
                    if (closed.Contains(nextState))
                        continue;

                    int gNew = currentNode.G + 1;
                    int hNew = Heuristic(nextState);

                    // Ha már nyitott listában van, és ott jobb G-je van, kihagyjuk
                    var existing = open.FirstOrDefault(n => n.State.Equals(nextState));
                    if (existing != null && existing.G <= gNew)
                        continue;

                    // Egyébként adjuk a nyitotthoz (vagy cseréljük)
                    if (existing != null)
                        open.Remove(existing);

                    open.Add(new SearchNode(nextState, currentNode, move, gNew, hNew));
                }
            }

            // nincs út
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