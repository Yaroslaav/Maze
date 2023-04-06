using System.Diagnostics;
using System.Numerics;
using System.Text;

public class Game
{
    private bool isPlaying = true;

    private int width = 9;
    private int height = 9;

    private int XStartPosition = 0;
    private int YStartPosition = 0;

    private int XCurrentPosition = 0;
    private int YCurrentPosition = 0;

    private int XFinishPosition = 9;
    private int YFinishPosition = 14;

    private Cell[,] cells;

    Stopwatch stopWatch = new Stopwatch();
    private int maxTimeInSeconds = 60;
    private int lastBeepSecond;

    Random rnd = new Random();

    ConsoleKey? lastKey = null;

    public void Start()
    {
        Console.CursorVisible = false;
        cells = new Cell[height, width];

        GenerateField();

        stopWatch.Reset();

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
            CheckButtons();
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
        int totalSeconds = maxTimeInSeconds - Convert.ToInt32(stopWatch.Elapsed.TotalSeconds);
        if(totalSeconds == 0)
        {
            Stop();
            return;
        }
        if(lastBeepSecond > totalSeconds)
        {
            TimerBeeps(totalSeconds);
        }

        int minutes = (totalSeconds - totalSeconds % 60) / 60;
        int seconds = totalSeconds % 60;
        string time = minutes + ":" + seconds;

        Console.WriteLine($"Залишилось часу: {time} ");
        lastBeepSecond = totalSeconds;
    }

    private void TimerBeeps(int seconds)
    {

    }

    public void UpdateFieldOnScreen()
    {
        Console.SetCursorPosition(0, 1);
        StringBuilder sb = new StringBuilder();

        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                switch(cells[y, x].type)
                {
                    case CellType.Wall:
                        sb.Append("#");
                        break;
                    case CellType.Player:
                        sb.Append("@");
                        break;
                    case CellType.Finish:
                        sb.Append("_");
                        break;
                    default:
                        sb.Append(".");
                        break;
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

    public void MoveCharacter(Direction direction)
    {
        int YnewPosition = YCurrentPosition;
        int XnewPosition = XCurrentPosition;

        switch (direction)
        {
            case Direction.Left:
                XnewPosition = XnewPosition - 1 < 0 ? XnewPosition : XnewPosition - 1;
                
                break;
            case Direction.Right:
                XnewPosition = XnewPosition + 1 > width - 1 ? XnewPosition : XnewPosition + 1;

                break;
            case Direction.Up:
                YnewPosition = YnewPosition - 1 < 0 ? YnewPosition : YnewPosition - 1;

                break;
            case Direction.Down:
                YnewPosition = YnewPosition + 1 > height - 1 ? YnewPosition : YnewPosition + 1;

                break;
        }
        switch(cells[YnewPosition, XnewPosition].type)
        {
            case CellType.Finish:
                Stop();
                return;
            case CellType.Wall:
                return;
        }
        cells[YCurrentPosition, XCurrentPosition].type = CellType.None;
        cells[YnewPosition, XnewPosition].type = CellType.Player;

        XCurrentPosition = XnewPosition;
        YCurrentPosition = YnewPosition;

        UpdateFieldOnScreen();
    }
    public void GenerateField()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[y, x] = new Cell(x, y);
            }
        }

        Cell current = cells[1, 1];
        Stack<Cell> stack = new Stack<Cell>();

        current.Visited = true;

        do
        {
            List<Cell> neighbors = GetUnvisitedNeighbors(current);
            if (neighbors.Count > 0)
            {
                Cell chosen = neighbors[rnd.Next(neighbors.Count)];
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

        cells[YStartPosition, XStartPosition].type = CellType.Player;


        XFinishPosition = width - 2;
        YFinishPosition = height - 2;

        cells[YFinishPosition, XFinishPosition].type = CellType.Finish;


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
        currentCell.type = CellType.None;
        newCell.type = CellType.None;

        cells[(currentCell.Y + newCell.Y) >> 1, (currentCell.X + newCell.X) >> 1].type = CellType.None;
    }

    public List<Cell> GetUnvisitedNeighbors(Cell cell)
    {
        int x = cell.X;
        int y = cell.Y;
        List<Cell> neighbors = new List<Cell>();

        Cell top = GetCell(x, y - 2);
        if (top != null && !top.Visited)
        {
            neighbors.Add(top);
        }

        Cell right = GetCell(x + 2, y);
        if (right != null && !right.Visited)
        {
            neighbors.Add(right);
        }

        Cell bottom = GetCell(x, y + 2);
        if (bottom != null && !bottom.Visited)
        {
            neighbors.Add(bottom);
        }

        Cell left = GetCell(x - 2, y);
        if (left != null && !left.Visited)
        {
            neighbors.Add(left);
        }

        return neighbors;
    }


    public Cell GetCell(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return null;
        }
        return cells[y, x];
    }
    public void CheckButtons()
    {

        Direction direction = Direction.None;

        while (Console.KeyAvailable)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (lastKey == key.Key)
                return;
            direction = GetDirection(key);
            lastKey = key.Key;

        }


        if (direction != Direction.None)
        {
            MoveCharacter(direction);
        }   

        lastKey = null;


    }

    public Direction GetDirection(ConsoleKeyInfo key) => key.KeyChar switch
    {
        'w' => Direction.Up,
        's' => Direction.Down,
        'a' => Direction.Left,
        'd' => Direction.Right,
        _ => Direction.None,    
    };




}
public enum Direction
{
    None,
    Up,
    Down,
    Right,
    Left,
}


