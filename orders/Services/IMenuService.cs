using orders.Models;

namespace orders.Services;

public interface IMenuService
{
    // Returns the menu; if the service had to use the JSON fallback, UsedFallback will be true
    ValueTask<IReadOnlyList<MenuItem>> GetMenuAsync(CancellationToken ct = default);

    // Submit current order to backend (no-op for pure JSON fallback implementation)
    ValueTask<string?> PlaceOrderAsync(OrderState state, CancellationToken ct = default);

    // Indicates whether the last GetMenuAsync used the fallback source
    bool UsedFallback { get; }
}
