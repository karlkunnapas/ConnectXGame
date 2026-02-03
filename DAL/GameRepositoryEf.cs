using BLL;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class GameRepositoryEf: IRepository<GameBrain>
{
    private readonly AppDbContext _dbContext;
    
    public GameRepositoryEf(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Dictionary<int, (Guid, string)> List()
    {
        var res = new Dictionary<int, (Guid, string)>();

        var count = 1;
        
        foreach (var dbGame in _dbContext.Games)
        {
            var gameName = $"{dbGame.GameName} {dbGame.Player1Name} ({dbGame.Player1Type.ToString()}) vs {dbGame.Player2Name} ({dbGame.Player2Type.ToString()})";
            res.Add(count, (dbGame.Id, gameName));
            count++;
        }

        return res;

    }
    
    public async Task<Dictionary<int, (Guid, string)>> ListAsync()
    {
        var res = new Dictionary<int, (Guid, string)>();

        var count = 1;
        
        foreach (var dbGame in await _dbContext.Games.ToListAsync())
        {
            var gameName = $"{dbGame.GameName} {dbGame.Player1Name} ({dbGame.Player1Type.ToString()}) vs {dbGame.Player2Name} ({dbGame.Player2Type.ToString()})";
            res.Add(count, (dbGame.Id, gameName));
            count++;
        }

        return res;

    }

    public string Save(GameBrain data)
    {
        // Use the DTO to get the serializable data
        var gameData = new GameData(data);
            
            // 1. Check if the game is already saved (has a non-default Id)
            var existingGame = _dbContext.Games.FirstOrDefault(g => g.Id == data.Id);

            if (existingGame != null)
            {
                // UPDATE (Overwrite)
                existingGame.GameName = data.GetName();
                
                // Embedded Configuration Data (if the board size changed during the game, we save the current state)
                existingGame.BoardWidth = gameData.GameConfiguration.BoardWidth;
                existingGame.BoardHeight = gameData.GameConfiguration.BoardHeight;
                existingGame.WinCondition = gameData.GameConfiguration.WinCondition;
                existingGame.IsCylindrical = gameData.GameConfiguration.IsCylindrical;
                existingGame.Player1Type = gameData.GameConfiguration.P1Type;
                existingGame.Player2Type = gameData.GameConfiguration.P2Type;

                // Game State Data
                existingGame.Player1Name = gameData.Player1Name;
                existingGame.Player2Name = gameData.Player2Name;
                existingGame.NextMoveByX = gameData.NextMoveByX;
                existingGame.GameBoard = gameData.GameBoard;
            
                _dbContext.Games.Update(existingGame);
                _dbContext.SaveChanges();
                return existingGame.Id.ToString();
            }
            else
            {
                // INSERT (New Save)
                var newGame = new Game
                {
                    Id = data.Id,
                    GameName = data.GetName(),
                    
                    // Embedded Configuration Data
                    BoardWidth = gameData.GameConfiguration.BoardWidth,
                    BoardHeight = gameData.GameConfiguration.BoardHeight,
                    WinCondition = gameData.GameConfiguration.WinCondition,
                    IsCylindrical = gameData.GameConfiguration.IsCylindrical,
                    Player1Type = gameData.GameConfiguration.P1Type,
                    Player2Type = gameData.GameConfiguration.P2Type,

                    // Game State Data
                    Player1Name = gameData.Player1Name,
                    Player2Name = gameData.Player2Name,
                    NextMoveByX = gameData.NextMoveByX,
                    GameBoard = gameData.GameBoard, // JSON-converted by DbContext
                };

                _dbContext.Games.Add(newGame);
                _dbContext.SaveChanges();
                
                return newGame.Id.ToString();
            }
    }

    public GameBrain Load(Guid id)
    {
        var game = _dbContext.Games
            .FirstOrDefault(g => g.Id == id);

        if (game == null) throw new KeyNotFoundException($"Game with ID {id} not found.");
            
        // Map the embedded Configuration data back to the BLL GameConfiguration
        var config = new GameConfiguration
        {
            Id = Guid.Empty,
            BoardWidth = game.BoardWidth,
            BoardHeight = game.BoardHeight,
            WinCondition = game.WinCondition,
            IsCylindrical = game.IsCylindrical,
            P1Type = game.Player1Type,
            P2Type = game.Player2Type
        };

        // Create the serializable DTO from the game data
        var gameDataDTO = new GameData
        {
            Id = game.Id,
            Name = game.GameName,
            GameBoard = game.GameBoard,
            GameConfiguration = config,
            Player1Name = game.Player1Name,
            Player2Name = game.Player2Name,
            NextMoveByX = game.NextMoveByX,
        };

        // Reconstruct the GameBrain and set its tracking ID
        var brain = new GameBrain(gameDataDTO);
            
        return brain;
    }

    public void Delete(Guid id)
    {
        var entity = _dbContext.Games.Find(id);
            
        // Only proceed if the entity exists
        if (entity != null)
        {
            _dbContext.Games.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}