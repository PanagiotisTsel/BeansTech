using AllTheBeans.UI.Models;

namespace AllTheBeans.UI.Services;

public interface IBeanCatalog
{
    //provide json data in the frontend
    Task<IReadOnlyList<Bean>> GetAllAsync(CancellationToken cancellationToken  =  default);
    Task<Bean?> GetByIdAsync(string id, CancellationToken cancellationToken  =  default);
    Task<IReadOnlyList<Bean>> SearchAsync(string? query, string? colour, CancellationToken cancellationToken  =  default);
}

