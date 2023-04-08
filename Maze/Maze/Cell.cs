
public class Cell
{
    public int X { get; set; }
    public int Y { get; set; }

    public bool Visited { get; set; }
    public CellType type { get; set; }


    public Cell(int x, int y)
    {
        X = x;
        Y = y;
        Visited = false;
        type = CellType.Wall;
    }
}
public enum CellType
{
    None,
    Wall,
    Player,
    Finish,
    Coin,
}