using Google.Protobuf.WellKnownTypes;
using RestaurantOrderManager.Backend.Grpc;

namespace RestaurantOrderManager.UI.Servers.Services;

public sealed class OrdersAdminServiceGrpc(Menu.MenuClient client, IConfiguration cfg) : IOrdersAdminService
{
    public async ValueTask<IReadOnlyList<TableInfo>> ListTablesAsync(CancellationToken ct = default)
    {
        var resp = await client.GetTableListAsync(new GetTableListRequest(), cancellationToken: ct);
        // Return full proto objects so UI can access Id/Description/Seats
        return resp.Tables.ToList();
    }

    public async ValueTask<IReadOnlyList<AdminOrder>> GetLiveOrdersAsync(string tableId, CancellationToken ct = default)
    {
        var resp = await client.GetLiveOrdersForTableAsync(new GetLiveOrdersForTableRequest
        {
            TableId = tableId,
            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
        }, cancellationToken: ct);

        if (resp.OrderIds.Count == 0) return [];

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
