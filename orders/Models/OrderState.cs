using System.Text.Json.Serialization;

namespace orders.Models;

public sealed class OrderState
{
    public required string TableId { get; set; }
    public List<OrderLine> Lines { get; set; } = [];

    [JsonIgnore]
    public decimal Total => Lines.Sum(l => l.LineTotal);

    public void Add(MenuItem item, int qty = 1)
    {
        var line = Lines.FirstOrDefault(l => l.ItemId == item.Id);
        if (line is null)
        {
            Lines.Add(new OrderLine
            {
                ItemId = item.Id,
                Name = item.Name,
                UnitPrice = item.Price,
                Quantity = Math.Max(1, qty)
            });
        }
        else
        {
            line.Quantity += Math.Max(1, qty);
        }
    }

    public void Increment(string itemId)
    {
        var line = Lines.FirstOrDefault(l => l.ItemId == itemId);
        if (line != null) line.Quantity++;
    }

    public void Decrement(string itemId)
    {
        var line = Lines.FirstOrDefault(l => l.ItemId == itemId);
        if (line == null) return;
        line.Quantity--;
        if (line.Quantity <= 0)
        {
            Lines.Remove(line);
        }
    }

    public void Remove(string itemId)
    {
        var line = Lines.FirstOrDefault(l => l.ItemId == itemId);
        if (line != null)
        {
            Lines.Remove(line);
        }
    }

    public void Clear() => Lines.Clear();
}
