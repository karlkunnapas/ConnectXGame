using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class SavedGames : PageModel
{
    private readonly IRepository<GameBrain> _gameRepo;
    
    public SavedGames(IRepository<GameBrain> gameRepo)
    {
        _gameRepo = gameRepo;
    }

    public Dictionary<int, (Guid, string)> Games { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Games = await _gameRepo.ListAsync();
    }
}