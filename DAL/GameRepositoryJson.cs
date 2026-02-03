using System.Text.Json;
using BLL;

namespace DAL;

public class GameRepositoryJson: IRepository<GameBrain>
{
    public Dictionary<int, (Guid, string)> List()
    {
        var dir = FilesystemHelpers.GetGameDirectory();
        var res = new Dictionary<int, (Guid, string)>();

        var count = 0;
        foreach (var fullFileName in Directory.EnumerateFiles(dir))
        {  
            var fileName = Path.GetFileName(fullFileName);
            if (!fileName.EndsWith(".json"))
            {
                //Console.WriteLine("File not json: " + fileName);
                continue;
            }
            count++;
            fileName = Path.GetFileNameWithoutExtension(fileName);
            var splitted = fileName.Split("_");
            res.Add(count, (Guid.Parse(splitted.Last()), string.Join(" ", splitted[..^1])));
        }

        return res;
    }

    public async Task<Dictionary<int, (Guid, string)>> ListAsync()
    {
        return List();
    }
    
    private Dictionary<Guid, string> ListWithGuids()
    {
        var dir = FilesystemHelpers.GetGameDirectory();
        var res = new Dictionary<Guid, string>();
        
        foreach (var fullFileName in Directory.EnumerateFiles(dir))
        {  
            var fileName = Path.GetFileName(fullFileName);
            if (!fileName.EndsWith(".json"))
            {
                continue;
            }
            fileName = Path.GetFileNameWithoutExtension(fileName);
            var splitted = fileName.Split("_");
            res.Add(Guid.Parse(splitted.Last()), fileName);
        }

        return res;
    }
    
    public string Save(GameBrain game)
    {
        var fileName = "";
        
        var gameData = new GameData(game);
        var jsonStr = JsonSerializer.Serialize(gameData);
        fileName = $"{SanitizeFileName(game.GetName())}_{game.GetPlayer1Name()} ({game.GetPlayer1Type().ToString()}) vs {game.GetPlayer2Name()} ({game.GetPlayer2Type().ToString()})_{game.Id}" + ".json";
        var fullFileName = FilesystemHelpers.GetGameDirectory() + Path.DirectorySeparatorChar + fileName;
        
        Dictionary<Guid, string> gameNames = ListWithGuids();
        if (gameNames.ContainsKey(game.Id))
        {
            Delete(game.Id);
        }
        File.WriteAllText(fullFileName, jsonStr);
        
        return fileName;
    }

    public GameBrain Load(Guid id)
    {
        var games = ListWithGuids();
        if (!games.ContainsKey(id))
        {
            throw new KeyNotFoundException($"Configuration with ID {id} not found.");
        }
        var gameName = games[id];
        var jsonFileName = FilesystemHelpers.GetGameDirectory() + Path.DirectorySeparatorChar + gameName + ".json";
        var jsonText = File.ReadAllText(jsonFileName);
        var gameBrain = JsonSerializer.Deserialize<GameData>(jsonText);
        GameBrain loadedBrain = new GameBrain(gameBrain);

        return loadedBrain ?? throw new NullReferenceException("Json deserialization returned null. Data: " + jsonText);
    }

    public void Delete(Guid id)
    {
        var games = ListWithGuids();
        var gameName = games[id];
        var jsonFileName = FilesystemHelpers.GetGameDirectory() + Path.DirectorySeparatorChar + gameName + ".json";
        if (File.Exists(jsonFileName))
        {
            File.Delete(jsonFileName);
        }
        else
        {
            Console.WriteLine("File not found.");
        }
    }
    
    public static string SanitizeFileName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return string.Empty;
        }
        
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string[] validParts = name.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries);
        return string.Join("", validParts).Trim().Trim(['_']);
    }
}