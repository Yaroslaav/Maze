using System.Numerics;

public class Movement
{
    public Action<Vector2, Vector2> OnMove;
    ConsoleKey? lastKey = null;
    public void CheckButtons(Cell[,] cells, int XPosition, int YPosition, int width, int height)
    {
        int XLastPosition = XPosition;
        int YLastPosition = YPosition;

        

        while (Console.KeyAvailable)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (lastKey == key.Key)
                return;

            lastKey = key.Key;

            switch (key.KeyChar)
            {

                case 'w':
                    if(YPosition > 0)
                    {
                        if (!cells[YPosition - 1, XPosition].Wall)
                            YPosition--;
                    }
                    break;
                case 's':
                    if(YPosition < height - 1)
                    {
                        if (!cells[YPosition + 1, XPosition].Wall)
                            YPosition++;
                    }
                    break;
                case 'a':
                    if(XPosition > 0)
                    {
                        if (!cells[YPosition, XPosition - 1].Wall)
                            XPosition--;
                    }
                    break;
                case 'd':
                    if(XPosition < width - 1)
                    {
                        if (!cells[YPosition, XPosition + 1].Wall)
                            XPosition++;
                    }

                    break;
            }
        }

        if (YLastPosition != YPosition || XLastPosition != XPosition)
        {
            OnMove?.Invoke(new Vector2(XLastPosition, YLastPosition), new Vector2(XPosition, YPosition));
        }

        lastKey = null;


    }

}

 
