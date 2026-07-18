using System.Text.Json;
using AllTheBeans.UI.Models;

namespace AllTheBeans.UI.Services;


public sealed class JsonBeanCatalog : IBeanCatalog
{
    private readonly string _jsonPath;
    private IReadOnlyList<Bean>? _cache;
    private readonly SemaphoreSlim _lock  =  new(1, 1);

    public JsonBeanCatalog(string jsonPath)
    {
        _jsonPath  =  jsonPath;
    }


    public async Task<IReadOnlyList<Bean>> GetAllAsync(CancellationToken cancellationToken  =  default)
    {
        await EnsureLoadedAsync(cancellationToken);
        return _cache!;
    }


    public async Task<Bean?> GetByIdAsync(string _id, CancellationToken cancellationToken  =  default)
    {
        var all  =  await GetAllAsync(cancellationToken);
        //case sensitive and retun null if no match
        return all.FirstOrDefault(b  => string.Equals(b._id, _id, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IReadOnlyList<Bean>> SearchAsync(string? query, string? colour, CancellationToken cancellationToken  =  default)
    {
        var all  =  await GetAllAsync(cancellationToken);

        query  =  string.IsNullOrWhiteSpace(query) ? null : query.Trim();
        colour  =  string.IsNullOrWhiteSpace(colour) ? null : colour.Trim();

        IEnumerable<Bean> filtered  =  all;

        if (query is not null)
        {
            filtered  =  filtered.Where(b  =>
                b.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || (b.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) || (b.Country?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (colour is not null)
        {
            filtered  =  filtered.Where(b  => b.Colour !=  null && b.Colour.Equals(colour, StringComparison.OrdinalIgnoreCase));
        }

        return filtered
            .OrderBy(b  => b.Name, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private async Task EnsureLoadedAsync(CancellationToken cancellationToken)
    {
        if (_cache is not null) return;

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_cache is not null) return;

            if (!File.Exists(_jsonPath))
            {
                throw new FileNotFoundException($"json file not found at '{_jsonPath}'");
            }

            await using var stream  =  File.OpenRead(_jsonPath);

            var options  =  new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive  =  true
            };

            var beans  =  await JsonSerializer.DeserializeAsync<List<Bean>>(stream, options, cancellationToken);
            if (beans is null)
            {
                throw new InvalidDataException("Failed to deserialize beans JSON.");
            }

            _cache  =  beans
                .Select(b  => b with
                {
                    _id = string.IsNullOrWhiteSpace(b._id) ? null! : b._id,
                    Description  =  b.Description?.Trim()
                })
                .ToList();
        }
        finally
        {
            _lock.Release();
        }
    }
}

