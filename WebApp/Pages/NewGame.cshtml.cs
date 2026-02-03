using System.ComponentModel.DataAnnotations;
using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class NewGame : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepo;
    private readonly IRepository<GameBrain> _gameRepo;

    public NewGame(IRepository<GameConfiguration> configRepo, IRepository<GameBrain> gameRepo)
    {
        _configRepo = configRepo;
        _gameRepo = gameRepo;
    }
    
    public SelectList ConfigurationSelectList { get; set; } = default!;

    [BindProperty]
    public string ConfigId { get; set; } = default!;

    [BindProperty]
    [Length(1, 32)]
    public string GameName { get; set; } = default!;
    
    [BindProperty]
    [Length(1, 32)]
    public string Player1Name { get; set; } = default!;

    [BindProperty]
    [Length(1, 32)]
    public string Player2Name { get; set; } = default!;
    
    

    public async Task OnGetAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var data = await _configRepo.ListAsync();
        var data2 = data.Select(i => new
        {
            id = i.Value.Item1,
            value = i.Value.Item2
        }).ToList();
        
        ConfigurationSelectList = new SelectList(data2, "id", "value");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDataAsync();
            return Page();
        }
        
        var conf = _configRepo.Load(Guid.Parse(ConfigId));
        GameBrain gameBrain = new GameBrain(conf, Player1Name, Player2Name);
        gameBrain.SetName(GameName);
        
        _gameRepo.Save(gameBrain);
        
        // redirect to gameplay
        return RedirectToPage("./GamePlay", new { id = gameBrain.Id});
    }

}