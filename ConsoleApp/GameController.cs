using BLL;
using ConsoleUI;
using DAL;

namespace ConsoleApp;

public class GameController
{
    private GameBrain GameBrain { get; set; }

    public GameController(GameConfiguration configuration, string player1Name, string player2Name)
    {
        player1Name = player1Name == "" ? "Player 1" : player1Name;
        player2Name = player2Name == "" ? "Player 2" : player2Name;
        GameBrain = new GameBrain(configuration, player1Name, player2Name);
    }
    
    public GameController(GameBrain gameBrain)
    {
        GameBrain = gameBrain;
    }

    public GameBrain GetGameBrain() => GameBrain;

    public string DrawInitialBoard()
    {
        // Game loop logic here

        // get the player move
        // update gamebrain state
        // draw out the ui
        // when game over, stop

        if (GameBrain.IsCylindrical())
        {
            Ui.DrawCylindricalBoard(GameBrain.GetBoard());
        }
        else
        {
            Ui.DrawBoard(GameBrain.GetBoard());
        }

        Ui.ShowNextPlayer(GameBrain.IsNextPlayerX());
        return "";
    }

    public string GameLoop(string input)
    {
        // Game loop logic here

        // get the player move
        // update gamebrain state
        // draw out the ui
        var y = 0;
        if (int.TryParse(input, out var x) && GameBrain.BoardCoordinatesAreValid(x - 1))
        {
            y = GameBrain.ProcessMove(x - 1);
            
            if (GameBrain.IsCylindrical())
            {
                var winner = GameBrain.GetWinnerForCylinder(x - 1, y);
                if (winner != ECellState.Empty)
                {
                    Ui.ShowCylindricalWinner(winner, GameBrain.GetBoard());
                    return "W";
                }
                Console.Clear();
            }
            else
            {
                var winner = GameBrain.GetWinner(x - 1, y);
                if (winner != ECellState.Empty)
                {
                    Ui.ShowWinner(winner, GameBrain.GetBoard());
                    return "W";
                }
                Console.Clear();
            }
            
            if (GameBrain.IsDraw())
            {
                if (GameBrain.IsCylindrical())
                {
                    Ui.ShowCylindricalDraw(GameBrain.GetBoard());
                }
                else
                {
                    Ui.ShowDraw(GameBrain.GetBoard());
                }
                return "D";
            }
        }
        else
        {
            throw new ArgumentException("Invalid input. Please try again.");
        }
        
        if (GameBrain.IsCylindrical())
        {
            Ui.DrawCylindricalBoard(GameBrain.GetBoard());
        }
        else
        {
            Ui.DrawBoard(GameBrain.GetBoard());
        }
        Ui.ShowNextPlayer(GameBrain.IsNextPlayerX());
        
        return "";
    }
}