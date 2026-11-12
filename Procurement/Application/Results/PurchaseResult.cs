using CeTe.Procurement.Domain.Model.Aggregates;
namespace CeTe.Procurement.Application.Results;

public record PurchaseResult(Guid Id, string Status, DateTimeOffset CreatedAt, string SupplierName,
    IEnumerable<PurchaseOrderLine> Lines);
