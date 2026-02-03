using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class DeleteGame : PageModel
{
    private readonly IRepository<GameBrain> _gameRepo;

    public DeleteGame(IRepository<GameBrain> gameRepo)
    {
        _gameRepo = gameRepo;
    }

    public GameBrain Game { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Game = _gameRepo.Load(Guid.Parse(id));
        
        if (Game == null)
        {
            return NotFound();
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        Guid guid = Guid.Parse(id);
        Game = _gameRepo.Load(guid);
        
        if (Game != null)
        {
            _gameRepo.Delete(guid);
        }
        
        return RedirectToPage("./SavedGames");
    }
}