using CeTe.Shared.Domain.Model;

namespace CeTe.Sales.Domain.Model.Aggregates;

public sealed class Payment : TenantEntity
{
    public Guid SaleId
    {
        get; set;
    }
    public string Method { get; set; } = "CASH"; public decimal Amount
    {
        get; set;
    }
    public string Status { get; set; } = "PAID";
}
