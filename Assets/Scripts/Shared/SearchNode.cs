using Mesint_RollingCube_console;
public class SearchNode
{
    public BoardState State { get; }
    public SearchNode Parent { get; }
    public Move? MoveFromParent { get; }  // hogy tudd, milyen mozdulattal jöttél ide

    public int G { get; }  // eddig megtett lépések száma
    public int H { get; }  // heurisztikus becslés
    public int F => G + H;

    public SearchNode(BoardState state, SearchNode parent, Move? move, int g, int h)
    {
        State = state;
        Parent = parent;
        MoveFromParent = move;
        G = g;
        H = h;
    }
}
