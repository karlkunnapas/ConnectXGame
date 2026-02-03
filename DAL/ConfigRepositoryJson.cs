using System.Text.Json;
using BLL;

namespace DAL;

public class ConfigRepositoryJson : IRepository<GameConfiguration>
{
    
    public Dictionary<int, (Guid, string)> List()
    {
        var dir = FilesystemHelpers.GetConfigDirectory();
        var res = new Dictionary<int, (Guid, string)>();

        var count = 1;
        foreach (var fullFileName in Directory.EnumerateFiles(dir))
        {  
            var fileName = Path.GetFileName(fullFileName);
            if (!fileName.EndsWith(".json"))
            {
                continue;
            }

            fileName = Path.GetFileNameWithoutExtension(fileName);
            var splitted = fileName.Split("_");
            res.Add(count, (Guid.Parse(splitted.Last()), string.Join(" ", splitted[..^1])));
            count++;
        }

        return res;
    }
    
    public async Task<Dictionary<int, (Guid, string)>> ListAsync()
    {
        return List();
    }
    
    private Dictionary<Guid, string> ListWithGuids()
    {
        var dir = FilesystemHelpers.GetConfigDirectory();
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
    
    public string Save(GameConfiguration data)
    {
        Dictionary<Guid, string> configNames = ListWithGuids();
        if (configNames.ContainsKey(data.Id))
        {
            Delete(data.Id);
        }
        
        var jsonStr = JsonSerializer.Serialize(data);
        var fileName = "";
        
        if (data.IsCylindrical)
        {
            fileName = $"{SanitizeFileName(data.Name)}_{data.BoardWidth}x{data.BoardHeight}_win{data.WinCondition}_cylindrical_{data.P1Type.ToString()} vs {data.P2Type.ToString()}_{data.Id}" + ".json";
        } else
        {
            fileName = $"{SanitizeFileName(data.Name)}_{data.BoardWidth}x{data.BoardHeight}_win{data.WinCondition}_{data.P1Type.ToString()} vs {data.P2Type.ToString()}_{data.Id}" + ".json";
        }
        var fullFileName = FilesystemHelpers.GetConfigDirectory() + Path.DirectorySeparatorChar + fileName;
        File.WriteAllText(fullFileName, jsonStr);
        
        
        return fileName;
    }

    public GameConfiguration Load(Guid id)
    {
        var configs = ListWithGuids();
        if (!configs.ContainsKey(id))
        {
            throw new KeyNotFoundException($"Configuration with ID {id} not found.");
        }
        var configName = configs[id];
        var jsonFileName = FilesystemHelpers.GetConfigDirectory() + Path.DirectorySeparatorChar + configName + ".json";
        var jsonText = File.ReadAllText(jsonFileName);
        var conf = JsonSerializer.Deserialize<GameConfiguration>(jsonText);

        return conf ?? throw new NullReferenceException("Json deserialization returned null. Data: " + jsonText);
    }

    public void Delete(Guid id)
    {
        var configs = ListWithGuids();
        var configName = configs[id];
        var jsonFileName = FilesystemHelpers.GetConfigDirectory() + Path.DirectorySeparatorChar + configName + ".json";
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