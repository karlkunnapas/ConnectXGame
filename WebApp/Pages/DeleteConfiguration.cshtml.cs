using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class DeleteConfiguration : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepo;

    public DeleteConfiguration(IRepository<GameConfiguration> configRepo)
    {
        _configRepo = configRepo;
    }

    [BindProperty]
    public GameConfiguration Configuration { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Configuration = _configRepo.Load(Guid.Parse(id));
        
        if (Configuration == null)
        {
            return NotFound();
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        Guid guid = Guid.Parse(id);
        Configuration = _configRepo.Load(guid);
        
        if (Configuration != null)
        {
            _configRepo.Delete(guid);
        }
        
        return RedirectToPage("./Configurations");
    }
}