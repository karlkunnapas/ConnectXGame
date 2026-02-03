using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepo;
    private readonly IRepository<GameBrain> _gameRepo;
    
    public IndexModel(IRepository<GameConfiguration> configRepo, IRepository<GameBrain> gameRepo)
    {
        _configRepo = configRepo;
        _gameRepo = gameRepo;
    }

    public Dictionary<int, (Guid, string)> Configurations { get; set; } = default!;
    public Dictionary<int, (Guid, string)> Games { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Configurations = await _configRepo.ListAsync();
        Games = await _gameRepo.ListAsync();
    }

}