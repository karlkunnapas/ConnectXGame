using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using WebApp.Hubs;
using System.Threading.Tasks;

namespace WebApp.Pages;

public class GamePlay : PageModel
{
    private readonly IRepository<GameBrain> _gameRepo;
    private readonly IHubContext<GameHub> _hubContext;
    private readonly AI _ai = new AI();

    public GamePlay(IRepository<GameBrain> gameRepo, IHubContext<GameHub> hubContext)
    {
        _gameRepo = gameRepo;
        _hubContext = hubContext;
    }

    public string GameId { get; set; } = default!;
    public GameBrain GameBrain { get; set; } = default!;
    public ECellState Winner { get; set; } = ECellState.Empty;
    public bool isNextMoveByX { get; set; } = true;
    public string CurrentPlayer { get; set; } = "1";
    public bool IsConnected { get; set; } = true;
    public bool CanMakeMove { get; set; } = true;
    public string Player1Name { get; set; } = "Player 1";
    public string Player2Name { get; set; } = "Player 2";
    public bool IsMultiplayerMode { get; set; } = true; // Default to multiplayer mode

    public void OnGet(string id, int? player)
    {
        GameId = id;
        GameBrain = _gameRepo.Load(Guid.Parse(GameId));
        isNextMoveByX = GameBrain.IsNextPlayerX();

        // Determine current player based on game state and player type
        if (GameBrain.GetPlayer1Type() == EPlayerType.Ai)
        {
            // If Player 1 is AI, current player is always 2 (human)
            CurrentPlayer = "2";
        }
        else if (GameBrain.GetPlayer2Type() == EPlayerType.Ai)
        {
            // If Player 2 is AI, current player is always 1 (human)
            CurrentPlayer = "1";
        }
        else if (player.HasValue)
        {
            // Multiplayer mode - use provided player
            CurrentPlayer = player.Value.ToString();
        }
        else
        {
            // Default to Player 1
            CurrentPlayer = "1";
        }

        // Get player names
        Player1Name = GameBrain.GetPlayer1Name();
        Player2Name = GameBrain.GetPlayer2Name();

        // Set current player based on whose turn it is
        CanMakeMove = Winner == ECellState.Empty;
    }
    

    private bool IsGameOver()
    {
        return Winner != ECellState.Empty;
    }
    
}