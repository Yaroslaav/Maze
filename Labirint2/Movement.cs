using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class Movement
{
    public Action<Vector2, Vector2> OnMove;
    ConsoleKey? lastKey = null;
    public void CheckButtons(char[,] field, int XPosition, int YPosition, int width, int height)
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
                        if (field[YPosition - 1, XPosition] == '.' || field[YPosition - 1, XPosition] == '_')
                            YPosition--;
                    }
                    break;
                case 's':
                    if(YPosition < height - 1)
                    {
                        if (field[YPosition + 1, XPosition] == '.' || field[YPosition + 1, XPosition] == '_')
                            YPosition++;
                    }
                    break;
                case 'a':
                    if(XPosition > 0)
                    {
                        if (field[YPosition, XPosition - 1] == '.' || field[YPosition, XPosition - 1] == '_')
                            XPosition--;
                    }
                    break;
                case 'd':
                    if(XPosition < width - 1)
                    {
                        if (field[YPosition, XPosition + 1] == '.' || field[YPosition, XPosition + 1] == '_')
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

 
