using CeTe.Shared.Domain.Model;
namespace CeTe.Sales.Domain.Model.Aggregates;

public sealed class SalesReceipt : TenantEntity
{
    public Guid SaleId
    {
        get; private set;
    }
    public string Number { get; private set; } = "";
    public string CompanyName { get; private set; } = "";
    public string Ruc { get; private set; } = "";
    public string Address { get; private set; } = "";
    public string Cashier { get; private set; } = "";
    public DateTimeOffset IssuedAt { get; private set; } = DateTimeOffset.UtcNow;

    private SalesReceipt()
    {
    }

    public SalesReceipt(Sale sale, string companyName, string ruc, string address, string cashier)
    {
        SaleId = sale.Id;
        Number = $"DEMO-{sale.Id.ToString("N")[..12].ToUpperInvariant()}";
        CompanyName = companyName;
        Ruc = ruc;
        Address = address;
        Cashier = cashier;
        IssuedAt = sale.CreatedAt;
    }
}
