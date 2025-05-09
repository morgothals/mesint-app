using System;


namespace Mesint_RollingCube_console
{
    public class CubeState : IEquatable<CubeState>
    {
        public int X { get; }
        public int Y { get; }
        
        // Indexek: 0=Top,1=Bottom,2=Front,3=Back,4=Left,5=Right
        public FaceColor[] Faces { get; }

        public CubeState(int x, int y, FaceColor[] faces)
        {
            X = x; Y = y;
            Faces = (FaceColor[])faces.Clone();
        }

        // Klónozó konstruktor
        public CubeState(CubeState other)
            : this(other.X, other.Y, other.Faces) { }

        // Egy lépés alkalmazása (pozíció + forgatás)
        public CubeState ApplyMove(Move m)
        {
            // 1) új pozíció
            int nx = X, ny = Y;
            switch (m)
            {
                case Move.Up: ny++; break;
                case Move.Down: ny--; break;
                case Move.Left: nx--; break;
                case Move.Right: nx++; break;
            }

            // 2) arc-átfordítás
            var f = (FaceColor[])Faces.Clone();
            // pl. Up mozdulatra: Top→Front, Front→Bottom, Bottom→Back, Back→Top
            if (m == Move.Up)
            {
                RotateFaces(ref f, 0, 2, 1, 3);
            }
            else if (m == Move.Down)
            {
                RotateFaces(ref f, 0, 3, 1, 2);
            }
            else if (m == Move.Left)
            {
                RotateFaces(ref f, 0, 4, 1, 5);
            }
            else if (m == Move.Right)
            {
                RotateFaces(ref f, 0, 5, 1, 4);
            }

            return new CubeState(nx, ny, f);
        }

        // segéd: 4 arcot körbeforgat
        private static void RotateFaces(ref FaceColor[] f, int a, int b, int c, int d)
        {
            var tmp = f[a];
            f[a] = f[b];
            f[b] = f[c];
            f[c] = f[d];
            f[d] = tmp;
        }

        public bool Equals(CubeState other)
        {
            if (other is null) return false;
            if (X != other.X || Y != other.Y) return false;
            for (int i = 0; i < Faces.Length; i++)
                if (Faces[i] != other.Faces[i]) return false;
            return true;
        }

        public override bool Equals(object obj)
            => Equals(obj as CubeState);

        public override int GetHashCode()
        {
            int h = X * 397 ^ Y;
            foreach (var face in Faces)
                h = h * 31 + face.GetHashCode();
            return h;
        }
    }
}
