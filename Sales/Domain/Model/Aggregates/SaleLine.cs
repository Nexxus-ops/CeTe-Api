using CeTe.Shared.Domain.Model;

namespace CeTe.Sales.Domain.Model.Aggregates;

public sealed class SaleLine : TenantEntity
{
    public Guid SaleId
    {
        get; set;
    }
    public Guid ProductId
    {
        get; set;
    }
    public string Sku { get; set; } = ""; public string Name { get; set; } = ""; public decimal Quantity
    {
        get; set;
    }
    public decimal UnitPrice
    {
        get; set;
    }
}
