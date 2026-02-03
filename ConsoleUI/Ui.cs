using BLL;

namespace ConsoleUI;

public static class Ui
{

    public static void ShowNextPlayer(bool isNextPlayerX)
    {
        Console.WriteLine("Next Player: " + (isNextPlayerX ? "Red" : "Blue"));
    }

    public static void ShowWinner(ECellState winner, ECellState[,] gameBoard)
    {
        
        Console.Clear();
        DrawBoard(gameBoard);
        Console.WriteLine("Winner is: " + (winner == ECellState.RedWin ? "Red" : "Blue"));
    }
    
    public static void ShowCylindricalWinner(ECellState winner, ECellState[,] gameBoard)
    {
        
        Console.Clear();
        DrawCylindricalBoard(gameBoard);
        Console.WriteLine("Winner is: " + (winner == ECellState.RedWin ? "Red" : "Blue"));
    }

    public static void ShowDraw(ECellState[,] gameBoard)
    {
        Console.Clear();
        DrawBoard(gameBoard);
        Console.WriteLine("Game ended in a draw!");
    }
    
    public static void ShowCylindricalDraw(ECellState[,] gameBoard)
    {
        Console.Clear();
        DrawCylindricalBoard(gameBoard);
        Console.WriteLine("Game ended in a draw!");
    }

    public static void DrawBoard(ECellState[,] gameBoard)
    {
        Console.Write("   ");
        for (int x = 0; x < gameBoard.GetLength(0); x++)
        {
            Console.Write("|" + GetNumberRepresentation(x + 1));
        }
        Console.WriteLine();
        
        for (int y = 0; y < gameBoard.GetLength(1); y++)
        {
            for (int x = 0; x < gameBoard.GetLength(0); x++)
            {
                Console.Write("---+");
            }

            Console.WriteLine("---");
            
            Console.Write(GetNumberRepresentation(y + 1));

            for (int x = 0; x < gameBoard.GetLength(0); x++)
            {
                Console.Write("|");
                PrintColoredCell(gameBoard[x, y]);
            }

            Console.WriteLine();
        }

    }

    public static void DrawCylindricalBoard(ECellState[,] gameBoard)
    {
        int width = gameBoard.GetLength(0);
        int height = gameBoard.GetLength(1);
        
        Console.Write("   ");
        for (int i = 0; i < width; i++)
            Console.Write($"|{GetNumberRepresentation(i + 1)}");
        Console.Write("|");
        for (int i = 0; i < width; i++)
            Console.Write($"|{GetNumberRepresentation(i + 1)}");
        Console.WriteLine();

        // Top border
        Console.Write("    ");
        for (int i = 0; i < width * 2; i++)
            if (i == width)
            {
                Console.Write("+");
            }
            else
            {
                Console.Write("---+");
            }
        Console.Write("---");
        Console.WriteLine();

        // Each row
        for (int y = 0; y < height; y++)
        {
            Console.Write(GetNumberRepresentation(y + 1));

            // left half
            for (int x = 0; x < width; x++)
            {
                Console.Write("|");
                PrintColoredCell(gameBoard[x, y]);
            }

            Console.Write("|");

            // right half (repeat)
            for (int x = 0; x < width; x++)
            {
                Console.Write("|");
                PrintColoredCell(gameBoard[x, y]);
            }

            Console.WriteLine();
        
            // row separator
            Console.Write("    ");
            for (int i = 0; i < width * 2; i++)
                if (i == width)
                {
                    Console.Write("+");
                }
                else
                {
                    Console.Write("---+");
                }
            Console.Write("---");
            Console.WriteLine();
        }

        Console.ResetColor();
    }

    public static void DrawCylindricalBoard2(ECellState[,] gameBoard, bool isCylindrical)
    {
        Console.Write("   ");
        for (int x = 0; x < gameBoard.GetLength(0); x++)
        {
            Console.Write("|" + GetNumberRepresentation(x + 1));
        }
        Console.WriteLine();
        
        int spaces = 0;
        for (int y = 0; y < gameBoard.GetLength(1); y++)
        {
            int middle = (int) Math.Ceiling(gameBoard.GetLength(0) / 2.0);
            
            if (y > 0)
            {
                if (y > middle)
                {
                    Console.Write(String.Concat(Enumerable.Repeat(" ", --spaces)));
                }
                else
                {
                    Console.Write(String.Concat(Enumerable.Repeat(" ", spaces)));
                }
            }
            for (int x = 0; x < gameBoard.GetLength(0); x++)
            {
                if (x == 0)
                {
                    if (y > middle)
                    {
                        Console.Write("---/");
                    } else if (y <= middle && y != 0)
                    {
                        Console.Write(@"---\");
                    }
                    else
                    {
                        Console.Write("---+");
                    }
                }
                else
                {
                    Console.Write("---+");
                }
            }

            Console.WriteLine("---");
            
            if (y > middle)
            {
                Console.Write(String.Concat(Enumerable.Repeat(" ", --spaces)));
            }
            else
            {
                if (y > 0)
                {
                    spaces++;
                }
                Console.Write(String.Concat(Enumerable.Repeat(" ", spaces)));
                spaces++;
            }
            
            Console.Write(GetNumberRepresentation(y + 1));

            for (int x = 0; x < gameBoard.GetLength(0); x++)
            {
                if (y == middle)
                {
                    Console.Write("|" + GetCellRepresentation(gameBoard[x, y]));
                } else if (y > middle)
                {
                    Console.Write("/" + GetCellRepresentation(gameBoard[x, y]));
                } else if (y < middle)
                {
                    Console.Write(@"\" + GetCellRepresentation(gameBoard[x, y]));
                }
            }
            Console.WriteLine();
            if (y == gameBoard.GetLength(1) - 1)
            {
                Console.Write(String.Concat(Enumerable.Repeat(" ", --spaces)));
                
                for (int x = 0; x < gameBoard.GetLength(0); x++)
                {
                    
                    if (x == 0)
                    {
                        Console.Write("---/");
                    }
                    else
                    {
                        Console.Write("---+");
                    }
                }

                Console.WriteLine("---");
            
                Console.Write(String.Concat(Enumerable.Repeat(" ", --spaces)));
                
                Console.Write(GetNumberRepresentation(1));

                for (int x = 0; x < gameBoard.GetLength(0); x++)
                {
                    Console.Write("/" + GetCellRepresentation(gameBoard[x, 0]));
                }

                Console.WriteLine();

                
            }
        }
    }

    private static string GetNumberRepresentation(int number)
    {
        return " " + (number < 10 ? "0" + number : number.ToString());
    }

    private static string GetCellRepresentation(ECellState cellValue) =>
        cellValue switch
        {
            ECellState.Empty => "   ",
            ECellState.Red => " Red ",
            ECellState.Blue => " Blue ",
            ECellState.RedWin => "XXX",
            ECellState.BlueWin => "OOO",
            _ => " ? "
        };
    
    
    private static void PrintColoredCell(ECellState cellValue)
    {
        switch (cellValue)
        {
            case ECellState.Red:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" O ");
                break;
            case ECellState.Blue:
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(" O ");
                break;
            case ECellState.RedWin:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.Write("XXX");
                break;
            case ECellState.BlueWin:
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Write("OOO");
                break;
            default:
                Console.ResetColor();
                Console.Write("   ");
                break;
        }

        Console.ResetColor(); // reset after each cell so the borders stay normal
    }

}

                /*
                    | 01| 02| 03| 04\ 05| 06| 07| 08| 09| 10
                   ---+---+---+---\___________+---+---+---+---
                    01|   |   |   \   |   |   |   |   |   |   
                   ---+---+---+---\___+---+---+---+---+---+---
                    02|   |   |   \   |   |   |   |   |   |   
                   ---+---+---+---\---+---+---+---+---+---+---
                    03|   |   |   \   |   |   |   |   |   |   
                   ---+---+---+---\---+---+---+---+---+---+---
                    04|   |   |   \   |   |   |   |   |   |   
                   ---+---+---+---\---+---+---+---+---+---+---
                    05|   |   |   \   |   |   |   |   |   |   
                   ---+---+---+---\---+---+---+---+---+---+---
                    05|   |   |   \   |   |   |   |   |   |   
                    
                    
                      | 01| 02| 03| 04| 05| 06| 07| 08| 09| 10
                   ---+---+---+---+---+---+---+---+---+---+---
                    01\   \   \   \   \   \   \   \   \   \   
                    ---\---+---+---+---+---+---+---+---+---+---
                      02\   \   \   \   \   \   \   \   \   \   
                      ---\---+---+---+---+---+---+---+---+---+---
                        03\   \   \   \   \   \   \   \   \   \   
                        ---\---+---+---+---+---+---+---+---+---+---
                         04|   |   |   |   |   |   |   |   |   |   
                        ---|---+---+---+---+---+---+---+---+---+---
                        05/   /   /   /   /   /   /   /   /   /   
                      ---/---+---+---+---+---+---+---+---+---+---
                      06/   /   /   /   /   /   /   /   /   /   
                    ---/---+---+---+---+---+---+---+---+---+---
                    01/   /   /   /   /   /   /   /   /   /   
                    
                        | 01| 02| 03| 04| 05 |
                     +---+---+---+---+---+
                   01 \   |   |   |   |   /
                      \---+---+---+---+---/
                    02 \   |   |   |   |   /
                      \---+---+---+---+---/
                    03 |   |   |   |   |   |
                      /---+---+---+---+---\
                    04 /   |   |   |   |   \
                      /---+---+---+---+---\
                   05 /   |   |   |   |   \
                     +---+---+---+---+---+

                 */