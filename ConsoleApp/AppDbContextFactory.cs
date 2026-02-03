using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ConsoleApp;

public class AppDbContextFactory: IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        homeDirectory = homeDirectory + Path.DirectorySeparatorChar;
        var dbDirectory = FilesystemHelpers.GetDbDirectory() + Path.DirectorySeparatorChar;
        
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite($"Data Source={dbDirectory}app.db");
        return new AppDbContext(optionsBuilder.Options);
    }
}