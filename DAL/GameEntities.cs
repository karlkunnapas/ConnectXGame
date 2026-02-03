using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using BLL;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL
{
    // --- 1. CONFIGURATION ENTITY ---
    
    /// Represents a single saved configuration in the database.
    public class Configuration
    {
        public Guid Id { get; set; } 

        // Properties mapped directly from GameConfiguration
        [MaxLength(255)]
        public string Name { get; set; } = default!;
        public int BoardWidth { get; set; }
        public int BoardHeight { get; set; }
        public int WinCondition { get; set; }
        public bool IsCylindrical { get; set; }
        public EPlayerType Player1Type { get; set; }
        public EPlayerType Player2Type { get; set; }
    }
    
    // --- 2. GAME ENTITY (SAVED GAME STATE) ---
    /// Represents a single saved game instance (current progress).
    public class Game
    { 
        public Guid Id { get; set; }
        [MaxLength(255)]
        public string GameName { get; set; } = default!;

        // --- EMBEDDED CONFIGURATION DATA ---
        public int BoardWidth { get; set; }
        public int BoardHeight { get; set; }
        public int WinCondition { get; set; }
        public bool IsCylindrical { get; set; }
        public EPlayerType Player1Type { get; set; }
        public EPlayerType Player2Type { get; set; }
        // ---------------------------------------------------

        // Data from GameData DTO:
        [MaxLength(255)]
        public string Player1Name { get; set; } = default!;
        [MaxLength(255)]
        public string Player2Name { get; set; } = default!;
        public bool NextMoveByX { get; set; }
        
        // The game board data will be stored as a JSON string in the database.
        // The DbContext will handle the conversion.
        public ECellState[][] GameBoard { get; set; } = default!;
    }

    // --- 3. CONVERTER FOR COMPLEX DATA ---
    /// Converts ECellState[][] (jagged array) to a string (JSON) for SQLite storage.
    /// This is necessary because EF Core cannot map multi-dimensional or jagged arrays directly.
    public class ECellStateJaggedArrayConverter : ValueConverter<ECellState[][], string>
    {
        public ECellStateJaggedArrayConverter()
            : base(
                // Convert to Provider (C# to DB)
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                // Convert from Provider (DB to C#)
                v => JsonSerializer.Deserialize<ECellState[][]>(v, (JsonSerializerOptions?)null)!)
        {
        }
    }
}
