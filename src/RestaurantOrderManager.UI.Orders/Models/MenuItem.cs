namespace RestaurantOrderManager.UI.Orders.Models;

public sealed class MenuItem
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required decimal Price { get; set; }
    public string? Category { get; set; }
    public string? ThumbnailImageUrl { get; set; }
}
