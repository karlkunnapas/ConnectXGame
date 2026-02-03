using System.ComponentModel.DataAnnotations;
using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class EditConfiguration : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepo;
    public bool Editing = false;
    
    // Default values for placeholders
    public const int DefaultBoardWidth = 7;
    public const int DefaultBoardHeight = 6;
    public const int DefaultWinCondition = 4;
    public const string DefaultName = "Classical";

    public EditConfiguration(IRepository<GameConfiguration> configRepo)
    {
        _configRepo = configRepo;
    }

    [BindProperty]
    public string? ConfName { get; set; }

    [BindProperty]
    [Range(1, 20, ErrorMessage = "Board height must be between 1 and 20")]
    public int? ConfHeight { get; set; }
    
    [BindProperty]
    [Range(1, 20, ErrorMessage = "Board width must be between 1 and 20")]
    public int? ConfWidth { get; set; }

    [BindProperty]
    [Range(1, 20, ErrorMessage = "Win condition must be between 1 and 20")]
    public int? ConfWin { get; set; }
    
    [BindProperty]
    public bool ConfCylindrical { get; set; }
    
    [BindProperty]
    public EPlayerType ConfP1Type { get; set; } = EPlayerType.Human;
    
    [BindProperty]
    public EPlayerType ConfP2Type { get; set; } = EPlayerType.Human;
    
    [BindProperty]
    public Guid ConfId { get; set; }
    
    public async Task<IActionResult> OnGetAsync(string? id)
    {
        if (id != null)
        {
            var config = _configRepo.Load(Guid.Parse(id));
            ConfId = config.Id;
            ConfName = config.Name;
            ConfHeight = config.BoardHeight;
            ConfWidth = config.BoardWidth;
            ConfWin = config.WinCondition;
            ConfCylindrical = config.IsCylindrical;
            ConfP1Type = config.P1Type;
            ConfP2Type = config.P2Type;
            Editing = true;
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        
        // Create configuration with defaults for empty fields
        var configuration = new GameConfiguration
        {
            Id = ConfId == Guid.Empty ? Guid.NewGuid() : ConfId,
            Name = string.IsNullOrWhiteSpace(ConfName) ? DefaultName : ConfName,
            BoardWidth = ConfWidth ?? DefaultBoardWidth,
            BoardHeight = ConfHeight ?? DefaultBoardHeight,
            WinCondition = ConfWin ?? DefaultWinCondition,
            IsCylindrical = ConfCylindrical,
            P1Type = ConfP1Type,
            P2Type = ConfP2Type
        };
        
        _configRepo.Save(configuration);

        return RedirectToPage("./Configurations");
    }
}