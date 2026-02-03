using BLL;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class ConfigRepositoryEf: IRepository<GameConfiguration>
{
    private readonly AppDbContext _dbContext;
    
    public ConfigRepositoryEf(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Dictionary<int, (Guid, string)> List()
    {
        var res = new Dictionary<int, (Guid, string)>();

        var count = 1;
        
        foreach (var dbConf in _dbContext.Configurations)
        {
            var configName = $"{dbConf.Name} {dbConf.BoardWidth}x{dbConf.BoardHeight} win{dbConf.WinCondition} {dbConf.Player1Type.ToString()} vs {dbConf.Player2Type.ToString()}" + (dbConf.IsCylindrical ? " cylindrical" : "");
            res.Add(count, (dbConf.Id, configName));
            count++;
        }

        return res;

    }
    
    public async Task<Dictionary<int, (Guid, string)>> ListAsync()
    {
        var res = new Dictionary<int, (Guid, string)>();

        var count = 1;
        
        foreach (var dbConf in await _dbContext.Configurations.ToListAsync())
        {
            var configName = $"{dbConf.Name} {dbConf.BoardWidth}x{dbConf.BoardHeight} win{dbConf.WinCondition} {dbConf.Player1Type.ToString()} vs {dbConf.Player2Type.ToString()}" + (dbConf.IsCylindrical ? " cylindrical" : "" );
            res.Add(count, (dbConf.Id, configName));
            count++;
        }

        return res;

    }

    public string Save(GameConfiguration data)
    {
        var existingConfig = _dbContext.Configurations
            .FirstOrDefault(c => c.Name == data.Name);

        if (existingConfig == null)
        {
            // Create New
            var newConfig = new Configuration
            {
                Id = data.Id,
                Name = data.Name,
                BoardWidth = data.BoardWidth,
                BoardHeight = data.BoardHeight,
                WinCondition = data.WinCondition,
                IsCylindrical = data.IsCylindrical,
                Player1Type = data.P1Type,
                Player2Type = data.P2Type
            };
            _dbContext.Configurations.Add(newConfig);
            _dbContext.SaveChanges();
            return newConfig.Id.ToString();
        }
        else
        {
            // Update Existing
            existingConfig.Name = data.Name; 
            existingConfig.BoardWidth = data.BoardWidth;
            existingConfig.BoardHeight = data.BoardHeight;
            existingConfig.WinCondition = data.WinCondition;
            existingConfig.IsCylindrical = data.IsCylindrical;
            existingConfig.Player1Type = data.P1Type;
            existingConfig.Player2Type = data.P2Type;
                
            _dbContext.Configurations.Update(existingConfig);
            _dbContext.SaveChanges();
            return existingConfig.Id.ToString();
        }
    }

    public GameConfiguration Load(Guid id)
    {
        var entity = _dbContext.Configurations.Find(id);
        if (entity == null) throw new KeyNotFoundException($"Configuration with ID {id} not found.");

        // Map entity back to BLL object
        return new GameConfiguration
        {
            Id = entity.Id,
            Name = entity.Name,
            BoardWidth = entity.BoardWidth,
            BoardHeight = entity.BoardHeight,
            WinCondition = entity.WinCondition,
            IsCylindrical = entity.IsCylindrical,
            P1Type = entity.Player1Type,
            P2Type = entity.Player2Type
        };
    }

    public void Delete(Guid id)
    {
        var entity = _dbContext.Configurations.Find(id);
            
        // Only proceed if the entity exists
        if (entity != null)
        {
            _dbContext.Configurations.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}