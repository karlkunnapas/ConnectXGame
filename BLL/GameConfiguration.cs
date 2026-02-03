using System.ComponentModel.DataAnnotations;

namespace BLL;

public class GameConfiguration() : BaseEntity
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = "Classical";
    private static int BoardWidthDefault { get; set; } = 7;
    private static int BoardHeightDefault { get; set; } = 6;
    private static int WinConditionDefault { get; set; } = 4;
    private static bool IsCylindricalDefault { get; set; } = false;
    private static EPlayerType P1TypeDefault { get; set; } = EPlayerType.Human;
    private static EPlayerType P2TypeDefault { get; set; } = EPlayerType.Human;
    
    [Range(1, 20, ErrorMessage = "Board width must be between 1 and 20")]
    public int BoardWidth { get; set; } = BoardWidthDefault;
    
    [Range(1, 20, ErrorMessage = "Board height must be between 1 and 20")]
    public int BoardHeight { get; set; } = BoardHeightDefault;
    
    [Range(1, 20, ErrorMessage = "Win condition must be between 1 and 20")]
    public int WinCondition { get; set; } = WinConditionDefault;
    
    public bool IsCylindrical { get; set; }

    public EPlayerType P1Type { get; set; } = P1TypeDefault;
    public EPlayerType P2Type { get; set; } = P2TypeDefault;


    public bool GameConfigEdit(string nameChoice, string widthChoice, string heightChoice, string winChoice,
        string cylindricalChoice, string player1Choice, string player2Choice)
    {
        widthChoice = widthChoice == "" ? BoardWidthDefault.ToString() : widthChoice;
        heightChoice = heightChoice == "" ? BoardHeightDefault.ToString() : heightChoice;
        winChoice = winChoice == "" ? WinConditionDefault.ToString() : winChoice;
        cylindricalChoice = (cylindricalChoice == "" || cylindricalChoice == "n")
            ? IsCylindricalDefault.ToString()
            : cylindricalChoice;
        player1Choice = player1Choice == "" ? "1" : player1Choice;
        player2Choice = player2Choice == "" ? "1" : player2Choice;

        if (IsValid(widthChoice, heightChoice, winChoice, cylindricalChoice, player1Choice, player2Choice, out var width, out var height, out var win, out var p1, out var p2))
        {
            Name = nameChoice;
            var isCylindrical = cylindricalChoice == "y";
            BoardWidth = width;
            BoardHeight = height;
            WinCondition = win;
            IsCylindrical = isCylindrical;
            P1Type = p1 == 1 ? EPlayerType.Human : EPlayerType.Ai;
            P2Type = p2 == 1 ? EPlayerType.Human : EPlayerType.Ai;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool GameConfigEditWithoutName(string widthChoice, string heightChoice, string winChoice,
        string cylindricalChoice, string player1Choice, string player2Choice)
    {
        widthChoice = widthChoice == "" ? BoardWidthDefault.ToString() : widthChoice;
        heightChoice = heightChoice == "" ? BoardHeightDefault.ToString() : heightChoice;
        winChoice = winChoice == "" ? WinConditionDefault.ToString() : winChoice;
        cylindricalChoice = (cylindricalChoice == "" || cylindricalChoice == "n")
            ? IsCylindricalDefault.ToString()
            : cylindricalChoice;
        player1Choice = player1Choice == "" ? "1" : player1Choice;
        player2Choice = player2Choice == "" ? "1" : player2Choice;

        if (IsValid(widthChoice, heightChoice, winChoice, cylindricalChoice, player1Choice, player2Choice, out var width, out var height, out var win, out var p1, out var p2))
        {
            var isCylindrical = cylindricalChoice == "y";
            BoardWidth = width;
            BoardHeight = height;
            WinCondition = win;
            IsCylindrical = isCylindrical;
            P1Type = p1 == 1 ? EPlayerType.Human : EPlayerType.Ai;
            P2Type = p2 == 1 ? EPlayerType.Human : EPlayerType.Ai;
            return true;
        }
        else
        {
            return false;
        }
        
    }
    
    private bool IsValid(string widthChoice1, string heightChoice1, string winChoice1, string cylindricalChoice1, string player1Choice, string player2Choice, out int width, out int height, out int win, out int p1, out int p2)
    {
        height = 0;
        win = 0;
        p1 = 0;
        p2 = 0;
        return int.TryParse(widthChoice1, out width) && int.TryParse(heightChoice1, out height) &&
               int.TryParse(winChoice1, out win) && (cylindricalChoice1 == "y" || cylindricalChoice1 == "False") && int.TryParse(player1Choice, out p1) && int.TryParse(player2Choice, out p2) && p1 is >= 1 and <= 2 && p2 is >= 1 and <= 2 && width <= 20 && height <= 20;
    }
}