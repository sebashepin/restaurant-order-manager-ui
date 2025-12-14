using RestaurantOrderManager.Backend.Grpc;

namespace RestaurantOrderManager.UI.Servers.Services;

public interface IOrdersAdminService
{
    ValueTask<IReadOnlyList<string>> ListTablesAsync(CancellationToken ct = default);
    ValueTask<IReadOnlyList<AdminOrder>> GetLiveOrdersAsync(string tableId, CancellationToken ct = default);
    ValueTask<bool> UpdateOrderStatusAsync(string orderId, OrderStatus status, CancellationToken ct = default);
}

public sealed record AdminOrder(string OrderId, OrderStatus Status);
