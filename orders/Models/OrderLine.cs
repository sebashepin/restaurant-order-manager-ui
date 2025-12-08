namespace orders.Models;

public sealed class OrderLine
{
    public required string ItemId { get; set; }
    public required string Name { get; set; }
    public required decimal UnitPrice { get; set; }
    public int Quantity { get; set; } = 1;

    public decimal LineTotal => UnitPrice * Quantity;
}
