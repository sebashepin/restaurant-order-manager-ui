using System.Net.Http.Json;
using orders.Models;

namespace orders.Services;

public sealed class MenuServiceWwwroot(HttpClient http) : IMenuService
{
    private IReadOnlyList<MenuItem>? _cache;
    public bool UsedFallback { get; private set; }

    public async ValueTask<IReadOnlyList<MenuItem>> GetMenuAsync(CancellationToken ct = default)
    {
        if (_cache is { Count: > 0 }) return _cache;
        var items = await http.GetFromJsonAsync<List<MenuItem>>("menu.json", cancellationToken: ct)
                    ?? new List<MenuItem>();
        _cache = items;
        UsedFallback = true;
        return _cache;
    }

    public ValueTask<string?> PlaceOrderAsync(OrderState state, CancellationToken ct = default)
        => ValueTask.FromResult<string?>(null);
}
