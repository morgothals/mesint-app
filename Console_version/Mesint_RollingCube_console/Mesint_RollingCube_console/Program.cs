using CubeGridConsole;

namespace Mesint_RollingCube_console
{
    class Program
    {
        static void Main()
        {
            const int size = 7;
            // példa akadálytábla
            bool[,] obstacles = new bool[size, size];
            // itt definiálhatod a 10 rögzített akadályt (pl. random, vagy kézzel)
            obstacles[1, 2] = true;
            obstacles[0, 3] = true;
            obstacles[1, 2] = true;
            obstacles[5, 3] = true;
            obstacles[5, 2] = true;
            obstacles[1, 3] = true;
            obstacles[4, 2] = true;
            obstacles[1, 4] = true;
            obstacles[1, 6] = true;
            obstacles[5, 6] = true;
            // ... összesen 10 pozíció

            // kezdő kocka (például középen, piros hátul)
            var startFaces = new FaceColor[] {
                FaceColor.Blue, FaceColor.Blue,
                FaceColor.Blue, FaceColor.Red,
                FaceColor.Blue, FaceColor.Blue
            };
            var startCube = new CubeState(3, 3, startFaces);
            var goal = (X: 6, Y: 6);

            var startState = new BoardState(size, obstacles, startCube, goal);

            Console.WriteLine("Random solver próbálkozás...");
            var rndSolver = new RandomSolver();
            var rndResult = rndSolver.Solve(startState, 1000);
            Console.WriteLine(rndResult != null
                ? "Random solver siker!"
                : "Random solver megbukott.");

            Console.WriteLine("Backtrack solver próbálkozás...");
            var btSolver = new BacktrackSolver();
            var btResult = btSolver.Solve(startState, 20);
            Console.WriteLine(btResult != null
                ? "Backtrack megtalálta."
                : "Backtrack nem találta.");

            Console.WriteLine("DFS solver próbálkozás...");
            var dfsSolver = new DFSSolver();
            var dfsResult = dfsSolver.Solve(startState);
            Console.WriteLine(dfsResult != null
                ? "DFS megtalálta."
                : "DFS nem találta.");

            Console.WriteLine("Vége. Enter az kilépéshez.");
            Console.ReadLine();
        }
    }
}
