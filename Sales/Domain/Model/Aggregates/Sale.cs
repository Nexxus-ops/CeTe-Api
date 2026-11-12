using CeTe.Shared.Domain.Model;
using CeTe.Shared.Domain.ValueObjects;

namespace CeTe.Sales.Domain.Model.Aggregates;

public sealed class Sale : TenantEntity
{
    public string CustomerName { get; set; } = ""; public Guid WarehouseId
    {
        get; set;
    }
    public Guid UserId
    {
        get; set;
    }
    public decimal TotalAmount
    {
        get; private set;
    }
    public string Status { get; private set; } = "COMPLETED"; public string RequestHash { get; set; } = ""; public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public void Confirm(IEnumerable<SaleLine> lines)
    {
        var list = lines.ToList();
        Rules.Require(list.Count > 0, "The sale is empty.");
        Rules.Require(list.All(l => l.Quantity > 0 && l.UnitPrice >= 0), "Invalid sale lines.");
        TotalAmount = Rules.Money(list.Sum(l => l.Quantity * l.UnitPrice));
    }
    public void Cancel()
    {
        Rules.Require(Status == "COMPLETED", "Sale already cancelled.", 409);
        Status = "CANCELLED";
    }
}
