using orders.Models;

namespace orders.Services;

public interface IMenuService
{
    ValueTask<IReadOnlyList<MenuItem>> GetMenuAsync(CancellationToken ct = default);
}
