using System;

namespace BLL;

public class AI
{
    private const int MaxDepth = 6; // Maximum search depth for minimax
    private const int WinScore = 1000000; // Score for winning position
    private const int LoseScore = -1000000; // Score for losing position
    private const int DrawScore = 0; // Score for draw position

    /// <summary>
    /// Finds the best move for the AI using minimax with alpha-beta pruning
    /// </summary>
    /// <param name="gameBrain">Current game state</param>
    /// <returns>Column index (0-based) for the best move</returns>
    public int FindBestMove(GameBrain gameBrain)
    {
        var board = gameBrain.GetBoard();
        var config = gameBrain.GetConfiguration();
        var isCylindrical = config.IsCylindrical;
        
        // Determine AI player (Red or Blue)
        var aiPlayer = gameBrain.IsNextPlayerX() ? ECellState.Red : ECellState.Blue;
        
        // Get all valid moves
        var validMoves = GetValidMoves(board, config);
        
        if (validMoves.Count == 0)
            return 0; // No valid moves available
            
        if (validMoves.Count == 1)
            return validMoves[0]; // Only one valid move

        // Quick check for immediate win or block opponent's win
        foreach (var col in validMoves)
        {
            var newBoard = (ECellState[,])board.Clone();
            var row = MakeMove(newBoard, col, aiPlayer, config);
            
            var winner = isCylindrical
                ? CheckWinnerForCylinder(newBoard, col, row, config, aiPlayer)
                : CheckWinner(newBoard, col, row, config, aiPlayer);
                
            if (winner == aiPlayer)
                return col; // Immediate win
        }

        // Check if opponent has immediate win and block it
        var opponent = aiPlayer == ECellState.Red ? ECellState.Blue : ECellState.Red;
        foreach (var col in validMoves)
        {
            var newBoard = (ECellState[,])board.Clone();
            var row = MakeMove(newBoard, col, opponent, config);
            
            var winner = isCylindrical
                ? CheckWinnerForCylinder(newBoard, col, row, config, opponent)
                : CheckWinner(newBoard, col, row, config, opponent);
                
            if (winner == opponent)
                return col; // Block opponent's win
        }

        var bestMove = validMoves[0];
        var bestScore = int.MinValue;
        var alpha = int.MinValue;
        var beta = int.MaxValue;

        // Try each valid move
        foreach (var col in validMoves)
        {
            // Make the move on a copy of the board
            var newBoard = (ECellState[,])board.Clone();
            var row = MakeMove(newBoard, col, aiPlayer, config);
            
            // Check if this move wins the game
            var winner = isCylindrical 
                ? CheckWinnerForCylinder(newBoard, col, row, config, aiPlayer)
                : CheckWinner(newBoard, col, row, config, aiPlayer);
                
            if (winner == aiPlayer)
            {
                return col; // Immediate win
            }

            // Evaluate the position using minimax
            var score = Minimax(newBoard, 0, false, alpha, beta, config, 
                aiPlayer, isCylindrical);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = col;
            }

            alpha = Math.Max(alpha, bestScore);
            
            // Alpha-beta pruning
            if (alpha >= beta)
                break;
        }

        // Final validation: ensure the chosen move is actually valid
        // This prevents edge cases where the AI might return a full column
        if (bestMove >= 0 && bestMove < config.BoardWidth)
        {
            // Check if the column is actually playable
            if (board[bestMove, 0] == ECellState.Empty)
            {
                return bestMove;
            }
            else
            {
                // Column is full, find the first available valid move
                for (int x = 0; x < config.BoardWidth; x++)
                {
                    if (board[x, 0] == ECellState.Empty)
                    {
                        return x;
                    }
                }
            }
        }

        // If no valid moves found, return 0 (shouldn't happen if GetValidMoves works correctly)
        return 0;
    }

    /// <summary>
    /// Minimax algorithm with alpha-beta pruning
    /// </summary>
    private int Minimax(ECellState[,] board, int depth, bool isMaximizing, 
        int alpha, int beta, GameConfiguration config, ECellState aiPlayer, bool isCylindrical)
    {
        // Check for terminal states
        var result = EvaluateBoard(board, config, aiPlayer, isCylindrical);
        
        if (result != 0 || depth >= MaxDepth)
            return result;

        var opponent = aiPlayer == ECellState.Red ? ECellState.Blue : ECellState.Red;
        var currentPlayer = isMaximizing ? aiPlayer : opponent;
        
        var validMoves = GetValidMoves(board, config);
        
        if (validMoves.Count == 0)
            return DrawScore;

        if (isMaximizing)
        {
            var maxEval = int.MinValue;
            
            foreach (var col in validMoves)
            {
                var newBoard = (ECellState[,])board.Clone();
                var row = MakeMove(newBoard, col, aiPlayer, config);
                
                // Check if AI wins
                var winner = isCylindrical 
                    ? CheckWinnerForCylinder(newBoard, col, row, config, aiPlayer)
                    : CheckWinner(newBoard, col, row, config, aiPlayer);
                    
                if (winner == aiPlayer)
                    return WinScore - depth; // Prefer faster wins
                
                var eval = Minimax(newBoard, depth + 1, false, alpha, beta, 
                    config, aiPlayer, isCylindrical);
                    
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
                
                if (alpha >= beta)
                    break;
            }
            
            return maxEval;
        }
        else
        {
            var minEval = int.MaxValue;
            
            foreach (var col in validMoves)
            {
                var newBoard = (ECellState[,])board.Clone();
                var row = MakeMove(newBoard, col, opponent, config);
                
                // Check if opponent wins
                var winner = isCylindrical 
                    ? CheckWinnerForCylinder(newBoard, col, row, config, opponent)
                    : CheckWinner(newBoard, col, row, config, opponent);
                    
                if (winner == opponent)
                    return LoseScore + depth; // Prefer slower losses
                
                var eval = Minimax(newBoard, depth + 1, true, alpha, beta, 
                    config, aiPlayer, isCylindrical);
                    
                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);
                
                if (alpha >= beta)
                    break;
            }
            
            return minEval;
        }
    }

    /// <summary>
    /// Evaluates the board position for the given player
    /// </summary>
    private int EvaluateBoard(ECellState[,] board, GameConfiguration config, 
        ECellState aiPlayer, bool isCylindrical)
    {
        var opponent = aiPlayer == ECellState.Red ? ECellState.Blue : ECellState.Red;
        var width = config.BoardWidth;
        var height = config.BoardHeight;
        var winCondition = config.WinCondition;
        
        var aiScore = 0;
        var opponentScore = 0;

        // Check all possible winning lines
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if (board[x, y] == ECellState.Empty)
                    continue;

                var player = board[x, y];
                
                // Check horizontal (and cylindrical)
                var horizontalScore = EvaluateDirection(board, x, y, 1, 0, 
                    config, player, isCylindrical);
                if (player == aiPlayer) aiScore += horizontalScore;
                else opponentScore += horizontalScore;

                // Check vertical
                var verticalScore = EvaluateDirection(board, x, y, 0, 1, 
                    config, player, false); // Vertical never wraps
                if (player == aiPlayer) aiScore += verticalScore;
                else opponentScore += verticalScore;

                // Check diagonal (down-right)
                var diagonalScore1 = EvaluateDirection(board, x, y, 1, 1, 
                    config, player, isCylindrical);
                if (player == aiPlayer) aiScore += diagonalScore1;
                else opponentScore += diagonalScore1;

                // Check diagonal (up-right)
                var diagonalScore2 = EvaluateDirection(board, x, y, 1, -1, 
                    config, player, isCylindrical);
                if (player == aiPlayer) aiScore += diagonalScore2;
                else opponentScore += diagonalScore2;
            }
        }

        return aiScore - opponentScore;
    }

    /// <summary>
    /// Evaluates a specific direction for potential winning lines
    /// </summary>
    private int EvaluateDirection(ECellState[,] board, int startX, int startY, 
        int dirX, int dirY, GameConfiguration config, ECellState player, bool isCylindrical)
    {
        var width = config.BoardWidth;
        var height = config.BoardHeight;
        var winCondition = config.WinCondition;
        
        var score = 0;
        var consecutive = 0;
        var openEnds = 0;

        // Count consecutive pieces in this direction
        for (var i = 0; i < winCondition; i++)
        {
            var x = startX + i * dirX;
            var y = startY + i * dirY;

            // Handle cylindrical wrapping for X coordinate
            if (isCylindrical)
            {
                x = ((x % width) + width) % width;
            }

            // Check bounds
            if (y < 0 || y >= height || (!isCylindrical && (x < 0 || x >= width)))
                break;

            if (board[x, y] == player)
            {
                consecutive++;
            }
            else if (board[x, y] == ECellState.Empty)
            {
                if (consecutive > 0)
                    openEnds++;
                break;
            }
            else
            {
                break;
            }
        }

        // Calculate score based on consecutive pieces and open ends
        if (consecutive == winCondition)
            return WinScore;
        else if (consecutive == winCondition - 1 && openEnds > 0)
            return 1000; // Very high score for near-win
        else if (consecutive == winCondition - 2 && openEnds > 1)
            return 100; // Good position
        else if (consecutive > 1)
            return consecutive * 10; // Basic score for consecutive pieces

        return 0;
    }

    /// <summary>
    /// Gets all valid moves (columns that are not full)
    /// </summary>
    private List<int> GetValidMoves(ECellState[,] board, GameConfiguration config)
    {
        var validMoves = new List<int>();
        var width = config.BoardWidth;
        var height = config.BoardHeight;

        for (var x = 0; x < width; x++)
        {
            // Check if top cell is empty (column not full)
            if (board[x, 0] == ECellState.Empty)
            {
                validMoves.Add(x);
            }
        }

        // Sort moves to prefer center columns (better performance)
        validMoves.Sort((a, b) => Math.Abs(a - width / 2).CompareTo(Math.Abs(b - width / 2)));

        return validMoves;
    }

    /// <summary>
    /// Makes a move on the board and returns the row where the piece was placed
    /// </summary>
    private int MakeMove(ECellState[,] board, int col, ECellState player, GameConfiguration config)
    {
        var height = config.BoardHeight;

        for (var y = 0; y < height; y++)
        {
            if (y == height - 1 || board[col, y + 1] != ECellState.Empty)
            {
                board[col, y] = player;
                return y;
            }
        }

        throw new InvalidOperationException("Invalid move - column is full");
    }

    /// <summary>
    /// Checks for winner in normal (non-cylindrical) board
    /// </summary>
    private ECellState CheckWinner(ECellState[,] board, int x, int y, GameConfiguration config, ECellState player)
    {
        var width = config.BoardWidth;
        var height = config.BoardHeight;
        var winCondition = config.WinCondition;

        // Check 4 directions: horizontal, vertical, diagonal-up, diagonal-down
        var directions = new[] { (1, 0), (0, 1), (1, 1), (1, -1) };

        foreach (var (dirX, dirY) in directions)
        {
            var count = 1;

            // Check positive direction
            for (var i = 1; i < winCondition; i++)
            {
                var nextX = x + i * dirX;
                var nextY = y + i * dirY;

                if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height)
                    break;

                if (board[nextX, nextY] == player)
                    count++;
                else
                    break;
            }

            // Check negative direction
            for (var i = 1; i < winCondition; i++)
            {
                var nextX = x - i * dirX;
                var nextY = y - i * dirY;

                if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height)
                    break;

                if (board[nextX, nextY] == player)
                    count++;
                else
                    break;
            }

            if (count >= winCondition)
                return player;
        }

        return ECellState.Empty;
    }

    /// <summary>
    /// Checks for winner in cylindrical board
    /// </summary>
    private ECellState CheckWinnerForCylinder(ECellState[,] board, int x, int y, GameConfiguration config, ECellState player)
    {
        var width = config.BoardWidth;
        var height = config.BoardHeight;
        var winCondition = config.WinCondition;

        // Check 4 directions: horizontal, vertical, diagonal-up, diagonal-down
        var directions = new[] { (1, 0), (0, 1), (1, 1), (1, -1) };

        foreach (var (dirX, dirY) in directions)
        {
            var count = 1;

            // Check positive direction with cylindrical wrapping
            for (var i = 1; i < winCondition; i++)
            {
                var nextX = x + i * dirX;
                var nextY = y + i * dirY;

                // Handle cylindrical wrapping for X coordinate
                nextX = ((nextX % width) + width) % width;

                if (nextY < 0 || nextY >= height)
                    break;

                if (board[nextX, nextY] == player)
                    count++;
                else
                    break;
            }

            // Check negative direction with cylindrical wrapping
            for (var i = 1; i < winCondition; i++)
            {
                var nextX = x - i * dirX;
                var nextY = y - i * dirY;

                // Handle cylindrical wrapping for X coordinate
                nextX = ((nextX % width) + width) % width;

                if (nextY < 0 || nextY >= height)
                    break;

                if (board[nextX, nextY] == player)
                    count++;
                else
                    break;
            }

            if (count >= winCondition)
                return player;
        }

        return ECellState.Empty;
    }
}