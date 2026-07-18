using System.ComponentModel.DataAnnotations;
using AllTheBeans.UI.Models;
using AllTheBeans.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AllTheBeans.UI.Pages;

public class OrderModel : PageModel
{
    private readonly IBeanCatalog _catalog;

    public OrderModel(IBeanCatalog catalog)
    {
        _catalog  =  catalog;
    }

    public SelectList BeanOptions { get; private set; }  =  new SelectList(Array.Empty<Models.Bean>(), "Id", "Name");

    [BindProperty(SupportsGet  =  true)]
    public string? BeanId { get; set; }

    [BindProperty]
    [Required(ErrorMessage  =  "Please enter your name for the order")]
    public string CustomerName { get; set; }  =  string.Empty;

    [BindProperty]
    [Range(1, 50, ErrorMessage  =  "must be between 1 and 50")]
    public int Quantity { get; set; }  =  1;

    [BindProperty]
    [Required(ErrorMessage  =  "select a bean")]
    public string SelectedBeanId { get; set; }  =  string.Empty;

    public string? ConfirmationMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        //selected bean
        var all  =  await _catalog.GetAllAsync(cancellationToken);
        BeanOptions  =  new SelectList(all, nameof(Models.Bean._id), nameof(Models.Bean.Name));

        if (!string.IsNullOrWhiteSpace(BeanId))
            SelectedBeanId  =  BeanId!;

        if (string.IsNullOrWhiteSpace(SelectedBeanId) && all.Count > 0)
            SelectedBeanId  =  all[0]._id;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        //submited
        var all  =  await _catalog.GetAllAsync(cancellationToken);
        BeanOptions  =  new SelectList(all, nameof(Models.Bean._id), nameof(Models.Bean.Name));

        if (!ModelState.IsValid)
            return Page();

        var bean  =  all.FirstOrDefault(b  => b._id  ==  SelectedBeanId);

        ConfirmationMessage  =  $"Thanks {CustomerName}. Your order for {Quantity} {bean.Name} is ready to be processed Total: {bean.Cost} × {Quantity}.";

        BeanOptions  =  new SelectList(all, nameof(Models.Bean._id), nameof(Models.Bean.Name), SelectedBeanId);

        return Page();
    }
}

