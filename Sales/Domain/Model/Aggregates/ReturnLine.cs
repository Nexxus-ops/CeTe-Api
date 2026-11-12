using CeTe.Shared.Domain.Model;

namespace CeTe.Sales.Domain.Model.Aggregates;

public sealed class ReturnLine : TenantEntity
{
    public Guid ReturnId
    {
        get; set;
    }
    public Guid SaleLineId
    {
        get; set;
    }
    public decimal Quantity
    {
        get; set;
    }
    public string Resolution { get; set; } = "";
}
