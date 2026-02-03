using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class Configurations : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepo;
    
    public Configurations(IRepository<GameConfiguration> configRepo)
    {
        _configRepo = configRepo;
    }

    public Dictionary<int, (Guid, string)> Configs { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Configs= await _configRepo.ListAsync();
    }
}