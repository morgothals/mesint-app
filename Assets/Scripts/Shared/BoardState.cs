using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mesint_RollingCube_console
{
    public class BoardState : IEquatable<BoardState>
    {
        public int GridSize { get; }
        public bool[,] Obstacles { get; }
        public CubeState Cube { get; }
        public (int X, int Y) Goal { get; }

        public BoardState(int gridSize, bool[,] obstacles, CubeState cube, (int, int) goal)
        {
            GridSize = gridSize;
            Obstacles = obstacles;
            Cube = cube;
            Goal = goal;
        }

        public bool IsGoal()
            => Cube.X == Goal.X && Cube.Y == Goal.Y;

        // Az érvényes mozgások listája
        public IEnumerable<Move> GetValidMoves()
        {
            foreach (Move m in Enum.GetValues(typeof(Move)).Cast<Move>())
            {
                var nextCube = Cube.ApplyMove(m);
                // határok + akadály + piros arc alul
                if (nextCube.X < 0 || nextCube.X >= GridSize ||
                    nextCube.Y < 0 || nextCube.Y >= GridSize)
                    continue;
                if (Obstacles[nextCube.X, nextCube.Y])
                    continue;
                // Faces[1] = Bottom
                if (nextCube.Faces[1] == FaceColor.Red)
                    continue;

                yield return m;
            }
        }

        // Új állapot a mozdulat után
        public BoardState Apply(Move m)
        {
            var newCube = Cube.ApplyMove(m);
            return new BoardState(GridSize, Obstacles, newCube, Goal);
        }

        public bool Equals(BoardState other)
        {
            if (other is null) return false;
            if (GridSize != other.GridSize ||
                !Cube.Equals(other.Cube) ||
                !Goal.Equals(other.Goal))
                return false;

            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                    if (Obstacles[x, y] != other.Obstacles[x, y])
                        return false;
            return true;
        }

        public override bool Equals(object obj)
            => Equals(obj as BoardState);

        public override int GetHashCode()
        {
            int h = GridSize * 397 ^ Cube.GetHashCode() ^ (Goal.X * 17 + Goal.Y);
            // csak egyszerűen vegyünk hash-t az akadályokról
            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                    if (Obstacles[x, y])
                        h = h * 23 + x * 7 + y;
            return h;
        }
    }
}