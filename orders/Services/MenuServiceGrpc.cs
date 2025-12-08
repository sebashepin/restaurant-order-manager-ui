using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client.Web;
using RestaurantOrderManager.Backend;
using UiMenuItem = orders.Models.MenuItem;
using orders.Models;

namespace orders.Services;

public sealed class MenuServiceGrpc(Menu.MenuClient client, MenuServiceWwwroot fallback) : IMenuService
{
    private IReadOnlyList<UiMenuItem>? _cache;
    public bool UsedFallback { get; private set; }

    public async ValueTask<IReadOnlyList<UiMenuItem>> GetMenuAsync(CancellationToken ct = default)
    {
        if (_cache is { Count: > 0 }) return _cache;
        try
        {
            var resp = await client.GetMenuAsync(new GetMenuRequest
            {
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
            }, cancellationToken: ct);

            _cache = resp.Items.Select(Map).ToList();
            UsedFallback = false;
            return _cache;
        }
        catch (Exception)
        {
            // Fallback to static JSON
            var items = await fallback.GetMenuAsync(ct);
            _cache = items;
            UsedFallback = true;
            return _cache;
        }
    }

    public async ValueTask<string?> PlaceOrderAsync(OrderState state, CancellationToken ct = default)
    {
        // If currently on fallback, there is no backend to send to
        if (UsedFallback)
        {
            return null;
        }

        try
        {
            var req = new PlaceOrderRequest
            {
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                TableId = state.TableId ?? string.Empty
            };
            req.Items.AddRange(state.Lines.Select(l => new OrderLineItem
            {
                ItemId = l.ItemId,
                Amount = l.Quantity
            }));

            var resp = await client.PlaceOrderAsync(req, cancellationToken: ct);
            return resp.OrderId;
        }
        catch (RpcException)
        {
            return null;
        }
    }

    private static UiMenuItem Map(RestaurantOrderManager.Backend.MenuItem m)
        => new UiMenuItem
        {
            Id = m.Id,
            Name = m.Name,
            Description = string.IsNullOrWhiteSpace(m.Description) ? null : m.Description,
            Price = (decimal)m.Price,
            Category = string.IsNullOrWhiteSpace(m.Category) ? null : m.Category,
            ThumbnailImageUrl = string.IsNullOrWhiteSpace(m.ThumbnailImageUrl) ? null : m.ThumbnailImageUrl
        };
}
