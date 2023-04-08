using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

#region enums
public enum Direction
{
    None,
    Up,
    Down,
    Right,
    Left,
    Horizontal,
    Vertical,

}
public enum GameState
{
    Playing,
    Shopping,
}
public enum ShopAbility
{
    None,
    MaxCointAmountIncrease,
    Exit,

}
#endregion

public class Game
{
    GameState state;

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
    private List<Cell> freeCells = new List<Cell>();

    private int maxCoinsAmount = 7;
    private int currentCoinsAmount;

    Stopwatch stopWatch = new Stopwatch();
    private int maxTimeInSeconds = 60;

    Random rnd = new Random();

    ConsoleKey? lastKey = null;

    public void Start()
    {
        SetupGame();

        SetGameState(GameState.Shopping);
        while (state == GameState.Shopping) 
        {
            CheckButtons();
        }

        GenerateField();

        stopWatch.Reset();
        Timer();

        UpdateFieldOnScreen();
        UpdateCoinCounter();

        isPlaying = true;

        GameLoop();
    }
    public void GameLoop()
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

        Save();

        isPlaying = false;
        Start();
    }

    public void SetupGame()
    {
        
        Console.CursorVisible = false;
        
        Load();
    }

    private void Timer()
    {
        Console.SetCursorPosition(0, 0);
        stopWatch.Start();

        int totalSeconds = maxTimeInSeconds - Convert.ToInt32(stopWatch.Elapsed.TotalSeconds);
        if(totalSeconds == 0)
        {
            Stop();
            return;
        }

        int minutes = (totalSeconds - totalSeconds % 60) / 60;
        int seconds = totalSeconds % 60;
        string time = minutes + ":" + seconds;

        Console.WriteLine($"Time left: {time}  ");
    }

    #region Updates
    public void UpdateFieldOnScreen()
    {
        Console.SetCursorPosition(0, 2);
        StringBuilder sb = new StringBuilder();

        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                switch(cells[y, x].type)
                {
                    case CellType.Wall:
                        sb.AppendFormat("\u001b[37m{0}","#");
                        break;
                    case CellType.Player:
                        sb.AppendFormat("\u001b[32m{0}", "@");
                        break;
                    case CellType.Finish:
                        sb.AppendFormat("\u001b[37m{0}", "_");
                        break;
                    case CellType.Coin:
                        sb.AppendFormat("\u001b[33m{0}", "0", "");
                        break;
                    default:
                        sb.AppendFormat("\u001b[37m{0}", ".");

                        break;
                }
            }
            sb.AppendFormat("\u001b[37m{0}", "#");
            sb.Append("\n");
        }
        for (int i = 0; i < width; i++)
        {
            sb.AppendFormat("\u001b[37m{0}", "#");
        }

        Console.WriteLine(sb);
    }
    public void UpdateCoinCounter()
    {
        Console.SetCursorPosition(0, 1);
        Console.WriteLine($"Coins: {currentCoinsAmount}"); 
    }
    public void UpdateShopInterface()
    {
        Console.Clear();
        StringBuilder sb = new StringBuilder();
        sb.Append($"Coins: {currentCoinsAmount}\n");
        sb.Append($"Amount of coins: {maxCoinsAmount}  |  price: 4 coins  |  to increase coins amount on field - press `1`\n");
        sb.Append($"To start - press `p`\n");
        Console.WriteLine(sb.ToString());
    }

    #endregion
    #region Shop
    public void SetupAbilitiesState()
    {
        UpdateShopInterface();
    }
    public void Buy(ShopAbility ability)
    {
        switch(ability)
        {
            case ShopAbility.MaxCointAmountIncrease:
                if (currentCoinsAmount < 4)
                    return;
                currentCoinsAmount -= 4;
                maxCoinsAmount++;
                break;
        }
        Save();
        UpdateShopInterface();
    }
    public void SetGameState(GameState _state)
    {
        switch(_state)
        {
            case GameState.Shopping:
                state = GameState.Shopping;
                SetupAbilitiesState();
                break;
            case GameState.Playing:
                state = GameState.Playing; 
                break;
        }
    }
    public void ShopKeys(ConsoleKeyInfo key)
    {
        switch(key.KeyChar)
        {
            case '1':
                Buy(ShopAbility.MaxCointAmountIncrease);
                break;
            case 'p':
                SetGameState(GameState.Playing);
                break;
        }
    }

    #endregion
    #region Ganeration
    public void GenerateField()
    {
        Console.Clear();
        cells = new Cell[height, width];

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
                freeCells.Add(chosen);
                stack.Push(current);
                current = chosen;
            }
            else if (stack.Count > 0)
            {
                current = stack.Pop();
            }
        } while (stack.Count != 0);
        

        XStartPosition = 1;
        YStartPosition = 1;

        ReverseCells();

        freeCells.Remove(cells[YStartPosition, XStartPosition]);
        freeCells.Remove(cells[YFinishPosition, XFinishPosition]);
        SpawnCoins();

        XFinishPosition = width - 2;
        YFinishPosition = height - 2;

        cells[YFinishPosition, XFinishPosition].type = CellType.Finish;

        XCurrentPosition = XStartPosition;
        YCurrentPosition = YStartPosition;

        cells[YStartPosition, XStartPosition].type = CellType.Player;

       

    }
    public void SpawnCoins()
    {
        int partOfFreeCells = freeCells.Count / maxCoinsAmount;
        for (int i = 0; i < maxCoinsAmount; i++)
        {
            int product = i * partOfFreeCells;

            Cell coinCell = freeCells[rnd.Next(product, product + partOfFreeCells)];
            cells[coinCell.Y, coinCell.X].type = CellType.Coin;
        }
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
    public Cell GetCell(int x, int y)
    {
        if (x < 0 || x >= width - 1 || y < 0 || y >= height - 1)
        {
            return null;
        }
        return cells[y, x];
    }
    #endregion
    #region Buttons\Movement
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
            case CellType.Coin:
                currentCoinsAmount++;
                break;
        }
        cells[YCurrentPosition, XCurrentPosition].type = CellType.None;
        cells[YnewPosition, XnewPosition].type = CellType.Player;

        XCurrentPosition = XnewPosition;
        YCurrentPosition = YnewPosition;

        UpdateFieldOnScreen();
        UpdateCoinCounter();
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
    public void CheckButtons()
    {
        switch (state)
        {
            case GameState.Playing:
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

                break;
            case GameState.Shopping:
                while (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    ShopKeys(key);
                }
                lastKey = null;
                break;
        }

    }
    public Direction GetDirection(ConsoleKeyInfo key) => key.KeyChar switch
    {
        'w' => Direction.Up,
        's' => Direction.Down,
        'a' => Direction.Left,
        'd' => Direction.Right,
        _ => Direction.None,    
    };
    #endregion
    #region SaveLoad
    const string GameFile = "./../../SaveMaze.txt";
    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = File.Create("savegame.bin"))
        {
            formatter.Serialize(stream, currentCoinsAmount);
            formatter.Serialize(stream, maxCoinsAmount);
        }
    }
    public void Load()
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = File.OpenRead("savegame.bin"))
            {
                currentCoinsAmount = (int)formatter.Deserialize(stream);
                maxCoinsAmount = (int)formatter.Deserialize(stream);
            }
        }catch (Exception exception)
        {
            Save();
        }

    }
    #endregion




}

