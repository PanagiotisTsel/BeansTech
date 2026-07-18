using AllTheBeans.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllTheBeans.UI.Pages;

public class BeanModel : PageModel
{
    private readonly IBeanCatalog _catalog;

    public BeanModel(IBeanCatalog catalog)
    {
        _catalog  =  catalog;
    }

    [BindProperty(SupportsGet  =  true)]
    public string? BeanId { get; set; }

    public Models.Bean? Bean { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(BeanId))
            return;
            
        Bean  =  await _catalog.GetByIdAsync(BeanId!.Trim(), cancellationToken);

    }
}

