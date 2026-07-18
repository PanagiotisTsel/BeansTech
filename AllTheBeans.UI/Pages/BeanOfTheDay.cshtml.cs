using AllTheBeans.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllTheBeans.UI.Pages;

public class BeanOfTheDayModel : PageModel
{
    private readonly IBeanCatalog _catalog;

    public BeanOfTheDayModel(IBeanCatalog catalog)
    {
        _catalog  =  catalog;
    }

    public Models.Bean? Bean { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var all  =  await _catalog.GetAllAsync(cancellationToken);
        Bean  =  all.FirstOrDefault(b  => b.IsBotD);
    }
}

