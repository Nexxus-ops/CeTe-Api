using CeTe.Shared.Domain.Model;
using CeTe.Shared.Domain.ValueObjects;
namespace CeTe.Sales.Domain.Model.Aggregates;

public sealed class Sale : TenantEntity
{
    public string CustomerName { get; private set; } = "";
    public Guid WarehouseId
    {
        get; private set;
    }
    public Guid UserId
    {
        get; private set;
    }
    public decimal TotalAmount
    {
        get; private set;
    }
    public string Status { get; private set; } = "COMPLETED";
    public string RequestHash { get; private set; } = "";
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    private Sale()
    {
    }

    public Sale(Guid id, string customerName, Guid warehouseId, Guid userId, string requestHash)
    {
        Rules.Require(id != Guid.Empty, "An idempotency identifier is required.");
        Id = id;
        CustomerName = string.IsNullOrWhiteSpace(customerName) ? "General customer" : customerName.Trim();
        WarehouseId = warehouseId;
        UserId = userId;
        RequestHash = requestHash;
        Status = "DRAFT";
    }

    public void Confirm(IEnumerable<SaleLine> lines)
    {
        Rules.Require(Status == "DRAFT", "Sale already confirmed.", 409);
        var details = lines.ToList();
        Rules.Require(details.Count > 0, "The sale is empty.");
        Rules.Require(details.All(line => line.SaleId == Id && line.Quantity > 0 && line.UnitPrice >= 0), "Invalid sale lines.");
        TotalAmount = Rules.Money(details.Sum(line => line.Quantity * line.UnitPrice));
        Status = "COMPLETED";
    }

    public void Cancel()
    {
        Rules.Require(Status == "COMPLETED", "Sale already cancelled.", 409);
        Status = "CANCELLED";
    }
}
