using Microsoft.AspNetCore.SignalR;
using BLL;
using DAL;

namespace WebApp.Hubs;

public class GameHub : Hub
{
    private readonly IRepository<GameBrain> _gameRepo;
    private static readonly List<ConnectedPlayer> _connectedPlayers = new();
    
    public GameHub(IRepository<GameBrain> gameRepo)
    {
        _gameRepo = gameRepo;
    }
    
    private class ConnectedPlayer
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string GameId { get; set; } = string.Empty;
        public string Player { get; set; } = string.Empty;
    }
    
    public async Task JoinGame(string gameId, string player)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        
        // Check if this game already has players
        var existingPlayers = _connectedPlayers.Where(p => p.GameId == gameId).ToList();
        
        if (existingPlayers.Count == 0)
        {
            // First player joining
            _connectedPlayers.Add(new ConnectedPlayer
            {
                ConnectionId = Context.ConnectionId,
                GameId = gameId,
                Player = player
            });
            
            await Clients.Group(gameId).SendAsync("GameJoined", player);
            
            // Check if there's an AI player in the game configuration
            var game = _gameRepo.Load(Guid.Parse(gameId));
            var hasAiPlayer = game.GetPlayer1Type() == EPlayerType.Ai || game.GetPlayer2Type() == EPlayerType.Ai;
            
            if (hasAiPlayer)
            {
                // Single-player AI game - no need to wait for another human player
                await Clients.Group(gameId).SendAsync("GameStatus", "Single-player AI game started!");
                
                // Send current turn status to the first player
                var nextPlayer = game.IsNextPlayerX() ? "1" : "2";
                await Clients.Caller.SendAsync("TurnChanged", nextPlayer);
            }
            else
            {
                // Human vs Human game - wait for another player
                await Clients.Group(gameId).SendAsync("GameStatus", "Waiting for another player...");
                
                // Send current turn status to the first player
                var nextPlayer = game.IsNextPlayerX() ? "1" : "2";
                await Clients.Caller.SendAsync("TurnChanged", nextPlayer);
            }
        }
        else if (existingPlayers.Count == 1)
        {
            // Second player joining - game becomes multiplayer
            _connectedPlayers.Add(new ConnectedPlayer
            {
                ConnectionId = Context.ConnectionId,
                GameId = gameId,
                Player = player
            });
            
            await Clients.Group(gameId).SendAsync("GameJoined", player);
            await Clients.Group(gameId).SendAsync("GameStatus", "Multiplayer game started!");
            
            // Send current turn status to both players
            var game = _gameRepo.Load(Guid.Parse(gameId));
            var nextPlayer = game.IsNextPlayerX() ? "1" : "2";
            
            // Send turn status to all players in the group
            await Clients.Group(gameId).SendAsync("TurnChanged", nextPlayer);
        }
        else
        {
            // Third+ player - spectator mode
            await Clients.Caller.SendAsync("GameError", "Game is full. You can watch as a spectator.");
        }
    }
    
    public async Task LeaveGame(string gameId, string player)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        
        // Remove from connected players list
        var connectedPlayer = _connectedPlayers.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId && p.GameId == gameId);
        if (connectedPlayer != null)
        {
            _connectedPlayers.Remove(connectedPlayer);
            
            // Notify other players
            var remainingPlayers = _connectedPlayers.Where(p => p.GameId == gameId).ToList();
            if (remainingPlayers.Count == 0)
            {
                await Clients.Group(gameId).SendAsync("GameStatus", "All players left. Game ended.");
            }
            else
            {
                await Clients.Group(gameId).SendAsync("GameLeft", player);
                await Clients.Group(gameId).SendAsync("GameStatus", $"{player} left the game. {remainingPlayers.Count} player(s) remaining.");
            }
        }
    }
    
    public async Task MakeMove(string gameId, int x, string playerId, bool isMultiplayer)
    {
        try
        {
            var game = _gameRepo.Load(Guid.Parse(gameId));
            
            // Check if this is a single-player game (has AI)
            var hasAiPlayer = game.GetPlayer1Type() == EPlayerType.Ai || game.GetPlayer2Type() == EPlayerType.Ai;
            
            // Only validate turn in multiplayer mode (no AI players)
            if (!hasAiPlayer && isMultiplayer)
            {
                // Validate it's the player's turn
                if ((playerId == "1" && !game.IsNextPlayerX()) ||
                    (playerId == "2" && game.IsNextPlayerX()))
                {
                    await Clients.Caller.SendAsync("InvalidMove", "Not your turn");
                    return;
                }
            }
            
            var y = game.ProcessMove(x);
            ECellState winner;
            
            if (game.IsCylindrical())
            {
                winner = game.GetWinnerForCylinder(x, y);
            }
            else
            {
                winner = game.GetWinner(x, y);
            }
            
            _gameRepo.Save(game);
            
            // Broadcast move to all players (including the player who made the move)
            await Clients.Group(gameId).SendAsync("ReceiveMove", x, y, playerId);
            
            if (winner != ECellState.Empty)
            {
                await Clients.Group(gameId).SendAsync("GameStatus", $"Game Over! {(winner == ECellState.RedWin ? game.GetPlayer1Name() : game.GetPlayer2Name())} wins!");
                await Clients.Group(gameId).SendAsync("GameOver", playerId);
                _gameRepo.Delete(Guid.Parse(gameId));
            }
            else if (game.IsDraw())
            {
                await Clients.Group(gameId).SendAsync("GameStatus", "Game Over! It's a draw!");
                await Clients.Group(gameId).SendAsync("GameOver", "D");
                _gameRepo.Delete(Guid.Parse(gameId));
            }
            else
            {
                // Update turn status
                var nextPlayer = game.IsNextPlayerX() ? "1" : "2";
                // Multiplayer human vs human game
                await Clients.Group(gameId).SendAsync("TurnChanged", nextPlayer);
            }
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("MoveError", ex.Message);
        }
    }

    public async Task MakeAiMove(string gameId, int x, string playerId)
    {
        try
        {
            var game = _gameRepo.Load(Guid.Parse(gameId));
            
            // If x is -1, calculate best move for AI
            if (x == -1)
            {
                var ai = new AI();
                x = ai.FindBestMove(game);
            }
            
            var y = game.ProcessMove(x);
            ECellState winner;
            
            if (game.IsCylindrical())
            {
                winner = game.GetWinnerForCylinder(x, y);
            }
            else
            {
                winner = game.GetWinner(x, y);
            }
            
            _gameRepo.Save(game);
            
            // Broadcast AI move to all players
            await Clients.Group(gameId).SendAsync("ReceiveAiMove", x, y, playerId);
            
            if (winner != ECellState.Empty)
            {
                await Clients.Group(gameId).SendAsync("GameStatus", $"Game Over! {(winner == ECellState.RedWin ? game.GetPlayer1Name() : game.GetPlayer2Name())} wins!");
                await Clients.Group(gameId).SendAsync("GameOver", playerId);
                _gameRepo.Delete(Guid.Parse(gameId));
            }
            else if (game.IsDraw())
            {
                await Clients.Group(gameId).SendAsync("GameStatus", "Game Over! It's a draw!");
                await Clients.Group(gameId).SendAsync("GameOver", "D");
                _gameRepo.Delete(Guid.Parse(gameId));
            }
            else
            {
                // Update turn status
                var nextPlayer = game.IsNextPlayerX() ? "1" : "2";
                await Clients.Group(gameId).SendAsync("GameStatus", $"Next turn: Player {nextPlayer}");
                await Clients.Group(gameId).SendAsync("TurnChanged", nextPlayer);
                
            }
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("MoveError", ex.Message);
        }
    }
}