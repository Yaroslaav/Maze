using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class Game
{
    private bool isPlaying = true;

    private char[,] field = new char[,] { };

    private int width = 11;
    private int height = 15;

    private int XStartPosition = 0;
    private int YStartPosition = 0;

    private int XCurrentPosition = 0;
    private int YCurrentPosition = 0;

    private int XFinishPosition = 9;
    private int YFinishPosition = 14;

    private Cell[,] cells;


    Movement movement = new Movement();
    Random rnd = new Random();

    public void Start()
    {

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
            movement.CheckButtons(field, XCurrentPosition, YCurrentPosition, width, height);
        }

    }
    public void Stop()
    {
        isPlaying = false;
        Start();
    }
    public void UpdateFieldOnScreen()
    {
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {

                /*if (cells[y, x].Visited)
                {
                    Console.Write(".");
                }
                else
                {
                    if (cells[y+1, x].TopWall)
                    {

                    }
                }*/
                if ((cells[y + 1, x].TopWall || cells[y, x + 1].LeftWall))
                {
                    Console.Write(".");
                }
                else
                {
                    Console.Write("#");
                }
            }
            Console.WriteLine();
        }
    }

    
    /*public void UpdateFieldOnScreen()
    {
        bool playerWasSet = false;
        //StringBuilder sb = new StringBuilder();
        for (int x = 0; x < width; x++)
        {
            Console.Write("#");
        }
        Console.Write("#\n");

        for (int y = 0; y < height; y++)
        {
            Console.Write("#");

            for (int x = 0; x < width; x++)
            {
                if (cells[y, x].BottomWall)
                {
                    if (x == XCurrentPosition && y == YCurrentPosition && !playerWasSet)
                    {
                        Console.Write("@");
                        playerWasSet = true;
                    }
                    else if (x == XFinishPosition && y == YFinishPosition)
                        Console.Write("_");
                    else
                        Console.Write("#");
                }
                else
                {
                    if (x == XCurrentPosition && y == YCurrentPosition && !playerWasSet)
                    {
                        Console.Write("@");
                        playerWasSet = true;
                    }
                    else if (x == XFinishPosition && y == YFinishPosition)
                        Console.Write("_");
                    else
                        Console.Write(".");
                }

                if (cells[y, x].RightWall)
                {
                    if (x == XCurrentPosition && y == YCurrentPosition && !playerWasSet)
                    {
                        Console.Write("@");
                        playerWasSet = true;
                    }
                    else if (x == XFinishPosition && y == YFinishPosition)
                        Console.Write("_");
                    else
                        Console.Write("#");
                }
                else
                {
                    if (x == XCurrentPosition && y == YCurrentPosition && !playerWasSet)
                    {
                        Console.Write("@");
                        playerWasSet = true;
                    }
                    else if (x == XFinishPosition && y == YFinishPosition)
                        Console.Write("_");
                    else
                        Console.Write(".");
                }
            }

            for (int x = 0; x < width; x++)
            {
                if (cells[y, x].TopWall)
                {
                    Console.Write("##");
                }
                else
                {
                    Console.Write("#.");
                }
            }
            Console.Write("\n");

            //sb.Append("#\n");
        }
        //sb.Remove(0, width*2+2);
        //sb.Remove(0, width*2+2);
        //sb.Remove(0, width*2+2);
        //sb.Insert(XCurrentPosition*YCurrentPosition,"@");
        //sb.Insert(XFinishPosition*YFinishPosition,"_");
        //Console.WriteLine(sb.ToString());


    }*/


    public void MoveCharacter(Vector2 lastPosition, Vector2 newPosition)
    {
        int YLastPosition = Convert.ToInt32(lastPosition.Y);
        int XLastPosition = Convert.ToInt32(lastPosition.X);

        int YnewPosition = Convert.ToInt32(newPosition.Y);
        int XnewPosition = Convert.ToInt32(newPosition.X);

        if (XnewPosition == XFinishPosition && YnewPosition == YFinishPosition)
        {
            Stop();
            return;
        }

        XCurrentPosition = XnewPosition;
        YCurrentPosition = YnewPosition;

        //field[YnewPosition, XnewPosition] = '@';
        //field[YLastPosition, XLastPosition] = '.';
        UpdateFieldOnScreen();
    }

    public void GenerateField()
    {
        /*char[,] _field = new char[height, width];


        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                _field[i, j] = '.';
            }

        }*/

        GenerateObstacles();

        //_field[YStartPosition, XStartPosition] = '@';
        //_field[YFinishPosition, XFinishPosition] = '_';

        //field = _field;
    }

    public void GenerateObstacles()
    {
        Cell current = cells[1, 1];
        current.Visited = true;
        Stack<Cell> stack = new Stack<Cell>();
        //return;
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
            //UpdateFieldOnScreen();
            //Thread.Sleep(10);
        } while (stack.Count != 0);


    }
    public void RemoveWall(Cell currentCell, Cell newCell)
    {
        int x = currentCell.X - newCell.X;
        int y = currentCell.Y - newCell.Y;

        if (x == 1)
        {
            currentCell.LeftWall = false;
            newCell.RightWall = false;
            /*
            cells[currentCell.Y - 1 < 0 ? 0 : currentCell.Y - 1, currentCell.X].BottomWall = false;
            cells[newCell.Y - 1 < 0 ? 0 : newCell.Y - 1, newCell.X].BottomWall = false;

            cells[currentCell.Y + 1 > height - 1 ? height - 1 : currentCell.Y + 1, currentCell.X].TopWall = false;
            cells[newCell.Y + 1 > height - 1 ? height - 1 : newCell.Y + 1, newCell.X].TopWall = false;
            */
        }
        else if (x == -1)
        {
            currentCell.RightWall = false;
            newCell.LeftWall = false;
            /*
            cells[currentCell.Y - 1 < 0 ? 0 : currentCell.Y - 1, currentCell.X].BottomWall = false;
            cells[newCell.Y - 1 < 0 ? 0 : newCell.Y - 1, newCell.X].BottomWall = false;

            cells[currentCell.Y + 1 > height - 1 ? height - 1 : currentCell.Y + 1, currentCell.X].TopWall = false;
            cells[newCell.Y + 1 > height - 1 ? height - 1 : newCell.Y + 1, newCell.X].TopWall = false;
            */
        }

        if (y == 1)
        {
            currentCell.TopWall = false;
            newCell.BottomWall = false;
            /*
            cells[currentCell.Y, currentCell.X - 1 < 0 ? 0 : currentCell.X - 1].RightWall = false;
            cells[newCell.Y, newCell.X - 1 < 0 ? 0 : newCell.X - 1].RightWall = false;

            cells[currentCell.Y, currentCell.X + 1 > width - 1 ? width - 1 : currentCell.X + 1].LeftWall = false;
            cells[newCell.Y, newCell.X + 1 > width - 1 ? width - 1 : newCell.X + 1].LeftWall = false;
        */
        }
        else if (y == -1)
        {
            currentCell.BottomWall = false;
            newCell.TopWall = false;
            /*
            cells[currentCell.Y, currentCell.X - 1 < 0 ? 0 : currentCell.X - 1].RightWall = false;
            cells[newCell.Y, newCell.X - 1 < 0 ? 0 : newCell.X - 1].RightWall = false;

            cells[currentCell.Y, currentCell.X + 1 > width - 1 ? width - 1 : currentCell.X + 1].LeftWall = false;
            cells[newCell.Y, newCell.X + 1 > width - 1 ? width - 1 : newCell.X + 1].LeftWall = false;
        */
        }
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
    public bool TopWall { get; set; }
    public bool BottomWall { get; set; }
    public bool LeftWall { get; set; }
    public bool RightWall { get; set; }

    public Cell(int x, int y)
    {
        X = x;
        Y = y;
        Visited = false;
        TopWall = true;
        BottomWall = true;
        //TopWall = true;
        //BottomWall = true;

        LeftWall = true;
        RightWall = true;
    }
}
