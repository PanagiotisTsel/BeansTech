using AllTheBeans.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllTheBeans.UI.Pages;

public class IndexModel : PageModel
{
    private readonly IBeanCatalog _catalog;

    public IndexModel(IBeanCatalog catalog)
    {
        _catalog  =  catalog;
    }

    [BindProperty(SupportsGet  =  true)]
    public string? Query { get; set; }

    [BindProperty(SupportsGet  =  true)]
    public string? Colour { get; set; }

    public IReadOnlyList<Models.Bean> Beans { get; set; }  =  Array.Empty<Models.Bean>();

    public IReadOnlyList<string> AvailableColours { get; set; }  =  Array.Empty<string>();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var all  =  await _catalog.GetAllAsync(cancellationToken);

        AvailableColours  =  all
            .Where(b  => !string.IsNullOrWhiteSpace(b.Colour))
            .Select(b  => b.Colour!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c  => c)
            .ToList();

        Beans  =  await _catalog.SearchAsync(Query, Colour, cancellationToken);
    }
}

