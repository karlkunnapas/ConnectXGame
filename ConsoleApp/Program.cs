using BLL;
using ConsoleApp;
using DAL;
using Microsoft.EntityFrameworkCore;


Console.WriteLine("Hello, CONNECTX!");

IRepository<GameConfiguration> configRepo;
IRepository<GameBrain> gameRepo;

Console.WriteLine();
Console.Write("Enter CRUD method ('j' for JSON, anything else for EF): ");
var methodChoice = Console.ReadLine();

using var dbContext = GetDbContext();
configRepo = new ConfigRepositoryEf(dbContext);
gameRepo = new GameRepositoryEf(dbContext);

if (methodChoice == "j")
{
    configRepo = new ConfigRepositoryJson();
    gameRepo = new GameRepositoryJson();
}

MenuFactory.CreateMenu(configRepo, gameRepo);


AppDbContext GetDbContext()
{
    // ========================= DB STUFF ========================
    var dbDirectory = FilesystemHelpers.GetDbDirectory() + Path.DirectorySeparatorChar;

    // We are using SQLite
    var connectionString = $"Data Source={dbDirectory}app.db";

    var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite(connectionString)
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging()
        //.LogTo(Console.WriteLine)
        .Options;

    var dbContext = new AppDbContext(contextOptions);
            
    // apply any pending migrations (recreates db as needed)
    dbContext.Database.Migrate();
            
    return dbContext;
}
