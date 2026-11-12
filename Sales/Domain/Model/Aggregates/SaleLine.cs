using CeTe.Shared.Domain.Model;
using CeTe.Shared.Domain.ValueObjects;
namespace CeTe.Sales.Domain.Model.Aggregates;

public sealed class SaleLine : TenantEntity
{
    public Guid SaleId
    {
        get; private set;
    }
    public Guid ProductId
    {
        get; private set;
    }
    public string Sku { get; private set; } = "";
    public string Name { get; private set; } = "";
    public decimal Quantity
    {
        get; private set;
    }
    public decimal UnitPrice
    {
        get; private set;
    }

    private SaleLine()
    {
    }

    public SaleLine(Guid saleId, Guid productId, string sku, string name, decimal quantity, Money unitPrice)
    {
        Rules.Require(quantity > 0, "Invalid sale lines.");
        SaleId = saleId;
        ProductId = productId;
        Sku = sku;
        Name = name;
        Quantity = quantity;
        UnitPrice = unitPrice.Amount;
    }
}
