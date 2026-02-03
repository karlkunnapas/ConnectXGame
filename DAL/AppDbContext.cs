using BLL;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext: DbContext
{ 
    public DbSet<Configuration> Configurations { get; set; } = default!;
    public DbSet<Game> Games { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder){
        base.OnModelCreating(modelBuilder);

        foreach (var relationship in modelBuilder.Model
                     .GetEntityTypes()
                     .Where(e => !e.IsOwned())
                     .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
        
        // Apply the custom converter to the GameBoard property
        modelBuilder
            .Entity<Game>()
            .Property(g => g.GameBoard)
            .HasConversion<ECellStateJaggedArrayConverter>();
        
        // Set up a unique index on the Configuration Name for fast lookup
        modelBuilder
            .Entity<Configuration>()
            .HasIndex(c => c.Name)
            .IsUnique();
    }
    
}