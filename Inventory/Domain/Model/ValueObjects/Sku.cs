using CeTe.Shared.Domain.ValueObjects;
namespace CeTe.Inventory.Domain.Model.ValueObjects;

public sealed record Sku
{
    public string Value
    {
        get;
    }
    public Sku(string value)
    {
        Rules.Require(!string.IsNullOrWhiteSpace(value), "SKU and name are required.");
        Value = value.Trim().ToUpperInvariant();
    }
}
