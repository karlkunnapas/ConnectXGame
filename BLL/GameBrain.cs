namespace BLL;

public class GameBrain: BaseEntity
{
    private string? Name { get; set; }
    private ECellState[,] GameBoard { get; set; }
    private GameConfiguration GameConfiguration  { get; set; }
    private string Player1Name { get; set; }
    private string Player2Name { get; set; }

    private bool NextMoveByX { get; set; } = true;
    
    public GameBrain(GameConfiguration configuration, string player1Name, string player2Name)
    {
        Player1Name = player1Name;
        Player2Name = player2Name;
        GameConfiguration = configuration;
        GameBoard = new ECellState[configuration.BoardWidth, configuration.BoardHeight];
    }
    
    public GameBrain(GameData data)
    {
        Id = data.Id;
        Name = data.Name;
        GameBoard = ToMultiDimensionalArray(data.GameBoard);
        GameConfiguration = data.GameConfiguration;
        Player1Name = data.Player1Name;
        Player2Name = data.Player2Name;
        NextMoveByX = data.NextMoveByX;
    }
    
    private ECellState[,] ToMultiDimensionalArray(ECellState[][] source)
    {
        if (source == null || source.Length == 0)
        {
            // Handle empty or invalid array gracefully
            return new ECellState[0, 0];
        }

        int rows = source.Length; // Width/X
        int cols = source[0].Length; // Height/Y
        
        ECellState[,] result = new ECellState[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = source[i][j];
            }
        }
        return result;
    }

    public void SetName(string name)
    {
        Name = name;
    }
    

    public string? GetName()
    {
        return Name;
    }

    public ECellState[,] GetBoard()
    {
        var gameBoardCopy = new ECellState[GameConfiguration.BoardWidth, GameConfiguration.BoardHeight];
        Array.Copy(GameBoard, gameBoardCopy, GameBoard.Length);
        return gameBoardCopy;
    }

    public bool IsNextPlayerX() => NextMoveByX;
    
    public GameConfiguration GetConfiguration() => GameConfiguration;
    public string GetPlayer1Name() => Player1Name;
    public string GetPlayer2Name() => Player2Name;
    public EPlayerType GetPlayer1Type() => GameConfiguration.P1Type;
    public EPlayerType GetPlayer2Type() => GameConfiguration.P2Type;
    public bool IsCylindrical() => GameConfiguration.IsCylindrical;
    
    

    /// <summary>
    /// Checks if the board is completely full (all cells occupied)
    /// </summary>
    public bool IsBoardFull()
    {
        for (int x = 0; x < GameConfiguration.BoardWidth; x++)
        {
            // If the top cell of any column is empty, the board is not full
            if (GameBoard[x, 0] == ECellState.Empty)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Checks if the game is a draw (board full with no winner)
    /// </summary>
    public bool IsDraw()
    {
        return IsBoardFull();
    }


    public int ProcessMove(int x)
    {
        for (int y = 0; y < GameBoard.GetLength(1); y++)
        {
            if ((y == GameConfiguration.BoardHeight - 1 || GameBoard[x, y + 1] != ECellState.Empty) && GameBoard[x, y] == ECellState.Empty)
            {
                GameBoard[x, y] = NextMoveByX ? ECellState.Red : ECellState.Blue;
                NextMoveByX = !NextMoveByX;
                return y;
            }
        }
        
        throw new ArgumentException("Invalid move.");
    }

    private (int dirX, int dirY) GetDirection(int directionIndex) =>
        directionIndex switch
        {
            0 => (-1, -1), // Diagonal up-left
            1 => (0, -1), // Vertical
            2 => (1, -1), // Diagonal up-right
            3 => (1, 0), // horizontal
            _ => (0, 0)
        };

    private (int dirX, int dirY) FlipDirection((int dirX, int dirY) direction) =>
        (-direction.dirX, -direction.dirY);

    public bool BoardCoordinatesAreValid(int x, int y = 0)
    {
        if (x < 0 || x > (GameConfiguration.BoardWidth - 1)) return false;
        if (y < 0 || y > (GameConfiguration.BoardHeight - 1)) return false;
        return true;
    }

    public ECellState GetWinner(int x, int y)
    {
        if (GameBoard[x, y] == ECellState.Empty) return ECellState.Empty;

        

        for (int directionIndex = 0; directionIndex < 4; directionIndex++)
        {
            var (dirX, dirY) = GetDirection(directionIndex);

            var count = 0;
            
            var nextX = x;
            var nextY = y;
            while (BoardCoordinatesAreValid(nextX, nextY) && GameBoard[x, y] == GameBoard[nextX, nextY] &&
                   count <= GameConfiguration.WinCondition)
            {
                count++;
                nextX += dirX;
                nextY += dirY;
            }

            if (count < GameConfiguration.WinCondition)
            {
                (dirX, dirY) = FlipDirection((dirX, dirY));
                nextX = x + dirX;
                nextY = y + dirY;
                while (BoardCoordinatesAreValid(nextX, nextY) && GameBoard[x, y] == GameBoard[nextX, nextY] &&
                       count <= GameConfiguration.WinCondition)
                {
                    count++;
                    nextX += dirX;
                    nextY += dirY;
                }
            }
            
            if (count == GameConfiguration.WinCondition)
            {
                return GameBoard[x, y] == ECellState.Red ? ECellState.RedWin : ECellState.BlueWin;
            }

        }


        return ECellState.Empty;
    }
    
    public ECellState GetWinnerForCylinder(int x, int y)
        {
            if (GameBoard[x, y] == ECellState.Empty) return ECellState.Empty;

            var playerState = GameBoard[x, y];
            var boardHeight = GameConfiguration.BoardHeight;
            var boardWidth = GameConfiguration.BoardWidth;

            for (int directionIndex = 0; directionIndex < 4; directionIndex++)
            {
                var (dirX, dirY) = GetDirection(directionIndex);
                var count = 0;

                var nextX = x;
                var nextY = y;
                
                // First, check in one direction (e.g., up-left)
                // The loop continues as long as Y is in bounds. X will wrap around.
                while (nextY >= 0 && nextY < boardHeight && count <= GameConfiguration.WinCondition)
                {
                    // Use modulo arithmetic to wrap the X coordinate
                    var wrappedX = (nextX % boardWidth + boardWidth) % boardWidth;

                    if (playerState == GameBoard[wrappedX, nextY])
                    {
                        count++;
                        nextX += dirX;
                        nextY += dirY;
                    }
                    else
                    {
                        break; // Stop if we find a non-matching piece
                    }
                }
                
                if (count < GameConfiguration.WinCondition)
                {
                    // If no win yet, check in the opposite direction and add to the count
                    (dirX, dirY) = FlipDirection((dirX, dirY));
                    nextX = x + dirX;
                    nextY = y + dirY;
                    
                    while (nextY >= 0 && nextY < boardHeight && count <= GameConfiguration.WinCondition)
                    {
                        var wrappedX = (nextX % boardWidth + boardWidth) % boardWidth;

                        if (playerState == GameBoard[wrappedX, nextY])
                        {
                            count++;
                            nextX += dirX;
                            nextY += dirY;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (count == GameConfiguration.WinCondition)
                {
                    return playerState == ECellState.Red ? ECellState.RedWin : ECellState.BlueWin;
                }
            }

            return ECellState.Empty;
        }
}
