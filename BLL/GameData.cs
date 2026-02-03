namespace BLL;

/// A Data Transfer Object (DTO) containing all the state needed to save and load a game.
/// Only public properties are used for easy serialization.
public class GameData
{
    // These properties are required to fully reconstruct the GameBrain instance.
    public ECellState[][] GameBoard { get; set; } = Array.Empty<ECellState[]>(); 
    public GameConfiguration GameConfiguration { get; set; } = new GameConfiguration();
    public Guid Id { get; set; } = Guid.Empty;
    public string? Name { get; set; } = string.Empty;
    public string Player1Name { get; set; } = string.Empty;
    public string Player2Name { get; set; } = string.Empty;
    public bool NextMoveByX { get; set; } = true;

    // Default constructor for the serializer
    public GameData() {}

    // Constructor to quickly pull data from an active GameBrain
    public GameData(GameBrain brain)
    {
        Id = brain.Id;
        Name = brain.GetName();
        GameBoard = ToJaggedArray(brain.GetBoard());
        GameConfiguration = brain.GetConfiguration();
        Player1Name = brain.GetPlayer1Name();
        Player2Name = brain.GetPlayer2Name();
        NextMoveByX = brain.IsNextPlayerX();
    }
    
    private ECellState[][] ToJaggedArray(ECellState[,] source)
    {
        int rows = source.GetLength(0); // Width/X
        int cols = source.GetLength(1); // Height/Y
        
        ECellState[][] result = new ECellState[rows][];

        for (int i = 0; i < rows; i++)
        {
            result[i] = new ECellState[cols];
            for (int j = 0; j < cols; j++)
            {
                result[i][j] = source[i, j];
            }
        }
        return result;
    }
}