using CeTe.Procurement.Domain.Model.Aggregates;
using CeTe.Procurement.Application.Results;
using CeTe.Procurement.Domain.Model.Commands;
using CeTe.Inventory.Application.Services;
namespace CeTe.Procurement.Application.Services;

public interface IProcurementCommandService
{
    Task<PurchaseOrder> Purchase(CreatePurchaseCommand input);
    Task<PurchaseOrder> Receive(Guid id, ReceivePurchaseCommand command);
}
