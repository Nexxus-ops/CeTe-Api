using CeTe.Shared.Domain.Model;

namespace CeTe.Sales.Domain.Model.Aggregates;

public sealed class SalesReceipt : TenantEntity
{
    public Guid SaleId
    {
        get; set;
    }
    public string Number { get; set; } = ""; public string CompanyName { get; set; } = ""; public string Ruc { get; set; } = ""; public string Address { get; set; } = ""; public string Cashier { get; set; } = ""; public DateTimeOffset IssuedAt { get; set; } = DateTimeOffset.UtcNow;
}
