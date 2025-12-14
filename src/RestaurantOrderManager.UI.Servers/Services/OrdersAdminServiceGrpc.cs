using Google.Protobuf.WellKnownTypes;
using RestaurantOrderManager.Backend.Grpc;

namespace RestaurantOrderManager.UI.Servers.Services;

public sealed class OrdersAdminServiceGrpc(Menu.MenuClient client, IConfiguration cfg) : IOrdersAdminService
{
    private IReadOnlyList<string>? _tablesCache;

    public ValueTask<IReadOnlyList<string>> ListTablesAsync(CancellationToken ct = default)
    {
        if (_tablesCache is { Count: > 0 }) return ValueTask.FromResult(_tablesCache);

        // Try to read configured tables; fallback to a sane default
        var configured = cfg.GetSection("Servers:Tables").Get<string[]>()
                        ?? (cfg["Servers:Tables"]?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? Array.Empty<string>());

        _tablesCache = (configured.Length > 0 ? configured : new[] { "A1", "A2", "B1", "B2" }).ToList();
        return ValueTask.FromResult(_tablesCache);
    }

    public async ValueTask<IReadOnlyList<AdminOrder>> GetLiveOrdersAsync(string tableId, CancellationToken ct = default)
    {
        var resp = await client.GetLiveOrdersForTableAsync(new GetLiveOrdersForTableRequest
        {
            TableId = tableId,
            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
        }, cancellationToken: ct);

        if (resp.OrderIds.Count == 0) return Array.Empty<AdminOrder>();

        var list = new List<AdminOrder>(resp.OrderIds.Count);
        foreach (var orderId in resp.OrderIds)
        {
            var st = await client.GetOrderStatusAsync(new GetOrderStatusRequest
            {
                OrderId = orderId,
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
            }, cancellationToken: ct);

            list.Add(new AdminOrder(orderId, st.Status));
        }

        return list;
    }

    public ValueTask<bool> UpdateOrderStatusAsync(string orderId, OrderStatus status, CancellationToken ct = default)
    {
        // NOTE: Backend RPC for updating order status not found in current proto (1.0.4).
        // Wire this up once available, for now return false to indicate no-op.
        return ValueTask.FromResult(false);
    }
}
