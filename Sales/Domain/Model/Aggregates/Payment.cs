using CeTe.Shared.Domain.Model;
using CeTe.Shared.Domain.ValueObjects;
namespace CeTe.Sales.Domain.Model.Aggregates;

public sealed class Payment : TenantEntity
{
    public Guid SaleId
    {
        get; private set;
    }
    public string Method { get; private set; } = "CASH";
    public decimal Amount
    {
        get; private set;
    }
    public string Status { get; private set; } = "PAID";

    private Payment()
    {
    }

    public Payment(Guid saleId, string method, Money amount)
    {
        Rules.Require(new[] { "CASH", "CARD", "TRANSFER" }.Contains(method), "Unsupported payment method.");
        SaleId = saleId;
        Method = method;
        Amount = amount.Amount;
    }

    public void Reverse() => Status = "REVERSED";
}
