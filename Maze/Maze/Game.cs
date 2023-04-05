using System.Diagnostics;
using System.Numerics;
using System.Text;

public class Game
{
    private bool isPlaying = true;

    private int width = 21;
    private int height = 25;

    private int XStartPosition = 0;
    private int YStartPosition = 0;

    private int XCurrentPosition = 0;
    private int YCurrentPosition = 0;

    private int XFinishPosition = 9;
    private int YFinishPosition = 14;

    private Cell[,] cells;

    Stopwatch stopWatch = new Stopwatch();

    Movement movement = new Movement();
    Random rnd = new Random();

    public void Start()
    {
        Console.CursorVisible = false;
        cells = new Cell[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[y, x] = new Cell(x, y);
            }
        }

        movement.OnMove += MoveCharacter;
        GenerateField();

        stopWatch.Restart();
        Timer();

        XCurrentPosition = XStartPosition;
        YCurrentPosition = YStartPosition;
        UpdateFieldOnScreen();

        isPlaying = true;
        Update();

    }
    public void Update()
    {
        while (isPlaying)
        {
            Timer();
            movement.CheckButtons(cells, XCurrentPosition, YCurrentPosition, width, height);
        }

    }
    public void Stop()
    {
        Console.Beep(1000, 200);

        isPlaying = false;
        Start();
    }

    private void Timer()
    {
        Console.SetCursorPosition(0, 0);
        stopWatch.Start();


        TimeSpan elapsed = stopWatch.Elapsed;
        Console.WriteLine($"Час раунду: {elapsed.Hours}:{elapsed.Minutes}:{elapsed.Seconds}");
    }

    public void UpdateFieldOnScreen()
    {
        Console.SetCursorPosition(0, 1);
        StringBuilder sb = new StringBuilder();

        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {

                if (cells[y, x].Wall)
                {
                    sb.Append("#");
                }
                else if (cells[y, x].Player)
                {
                    sb.Append("@");
                }
                else if (cells[y, x].Finish)
                {
                    sb.Append("_");
                }
                else
                {
                    sb.Append(".");
                }
            }
            sb.Append("#");
            sb.Append("\n");
        }
        for (int i = 0; i < width; i++)
        {
            sb.Append("#");
        }

        Console.WriteLine(sb);
    }

    public void MoveCharacter(Vector2 lastPosition, Vector2 newPosition)
    {
        int YnewPosition = Convert.ToInt32(newPosition.Y);
        int XnewPosition = Convert.ToInt32(newPosition.X);

        if (cells[YnewPosition, XnewPosition].Finish)
        {
            Stop();
            return;
        }

        cells[YCurrentPosition, XCurrentPosition].Player = false;
        cells[YnewPosition, XnewPosition].Player = true;

        XCurrentPosition = XnewPosition;
        YCurrentPosition = YnewPosition;

        UpdateFieldOnScreen();
    }
    public void GenerateField()
    {
        Cell current = cells[1, 1];
        Stack<Cell> stack = new Stack<Cell>();

        current.Visited = true;

        do
        {
            Cell[] neighbors = GetUnvisitedNeighbors(current);
            if (neighbors.Length > 0)
            {
                Cell chosen = neighbors[rnd.Next(neighbors.Length)];
                RemoveWall(current, chosen);
                chosen.Visited = true;
                stack.Push(current);
                current = chosen;
            }
            else if (stack.Count > 0)
            {
                current = stack.Pop();
            }
        } while (stack.Count != 0);

        ReverseCells();

        XStartPosition = 1;
        YStartPosition = 1;

        cells[YStartPosition, XStartPosition].Player = true;
        cells[YStartPosition, XStartPosition].Wall = false;


        XFinishPosition = width - 2;
        YFinishPosition = height - 2;

        cells[YFinishPosition, XFinishPosition].Finish = true;
        cells[YFinishPosition, XFinishPosition].Wall = false;


    }

    public void ReverseCells()
    {
        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1) / 2; j++)
            {

                Cell temp = cells[i, j];
                cells[i, j] = cells[i, cells.GetLength(1) - 1 - j];
                cells[i, cells.GetLength(1) - 1 - j] = temp;
            }
        }

        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1) / 2; j++)
            {
                Cell temp = cells[i, j];
                cells[i, j] = cells[cells.GetLength(0) - 1 - i, j];
                cells[cells.GetLength(0) - 1 - i, j] = temp;
            }
        }
    }

    public void RemoveWall(Cell currentCell, Cell newCell)
    {
        int x = currentCell.X - newCell.X;
        int y = currentCell.Y - newCell.Y;
        currentCell.Wall = false;
        newCell.Wall = false;

        cells[(currentCell.Y + newCell.Y) >> 1, (currentCell.X + newCell.X) >> 1].Wall = false;
    }

    public Cell[] GetUnvisitedNeighbors(Cell cell)
    {
        int x = cell.X;
        int y = cell.Y;
        Cell[] neighbors = new Cell[4];
        int count = 0;

        Cell top = GetCell(x, y - 2);
        if (top != null && !top.Visited)
        {
            neighbors[count++] = top;
        }

        Cell right = GetCell(x + 2, y);
        if (right != null && !right.Visited)
        {
            neighbors[count++] = right;
        }

        Cell bottom = GetCell(x, y + 2);
        if (bottom != null && !bottom.Visited)
        {
            neighbors[count++] = bottom;
        }

        Cell left = GetCell(x - 2, y);
        if (left != null && !left.Visited)
        {
            neighbors[count++] = left;
        }

        Array.Resize(ref neighbors, count);
        return neighbors;
    }
    public Cell[] GetUnvisitedCells()
    {
        Cell[] unvisited = new Cell[width * height];
        int count = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!cells[y, x].Visited)
                {
                    unvisited[count++] = cells[y, x];
                }
            }
        }

        Array.Resize(ref unvisited, count);
        return unvisited;
    }


    public Cell GetCell(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return null;
        }
        return cells[y, x];
    }





}
public class Cell
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool Visited { get; set; }
    public bool Wall { get; set; }
    public bool Player { get; set; }
    public bool Finish { get; set; }


    public Cell(int x, int y)
    {
        X = x;
        Y = y;
        Visited = false;
        Wall = true;
        Player = false;
        Finish = false;
    }
}
