using CeTe.Procurement.Application.Results;
using CeTe.Procurement.Domain.Model.Aggregates;
using CeTe.Procurement.Domain.Model.Commands;
using CeTe.Procurement.Domain.Repositories;
using CeTe.Shared.Application.Services;
using CeTe.Shared.Domain.Model;
using CeTe.Shared.Domain.Repositories;
using CeTe.Shared.Domain.ValueObjects;
using CeTe.Inventory.Application.Services;

using CeTe.Procurement.Application.Services;
namespace CeTe.Procurement.Application.Internal.CommandServices;

public sealed class ProcurementCommandService(IProcurementRepository repository, IUnitOfWork unitOfWork, ICurrentUser user, IInventoryFacade inventory) : IProcurementCommandService
{
    public Task<PurchaseOrder> Purchase(CreatePurchaseCommand input) => unitOfWork.Transaction<PurchaseOrder>(async () =>
    {
        await inventory.GetProduct(input.ProductId);
        Rules.Require(input.Quantity > 0 && input.UnitCost >= 0 && !string.IsNullOrWhiteSpace(input.SupplierName), "Invalid purchase.");
        var supplier = await repository.FindSupplier(input.SupplierName.Trim());
        if (supplier == null)
        {
            supplier = new Supplier { Name = input.SupplierName.Trim() };
            repository.AddSupplier(supplier);
        }
        var order = new PurchaseOrder(supplier.Id, (await inventory.GetWarehouse(input.WarehouseId)).Id);
        repository.AddOrder(order);
        repository.AddLine(new PurchaseOrderLine(order.Id, input.ProductId, input.Quantity, new Money(input.UnitCost)));
        return order;
    });
    public Task<PurchaseOrder> Receive(Guid id, ReceivePurchaseCommand command) => unitOfWork.Transaction<PurchaseOrder>(async () =>
    {
        var order = await repository.FindOrder(id) ?? throw new BusinessException("Resource not found.", 404);
        var line = await repository.OrderLine(id);
        order.Receive(line, command.Quantity);
        var receipt = new GoodsReceipt { PurchaseOrderId = id, UserId = user.UserId };
        repository.AddReceipt(receipt);
        repository.AddReceiptLine(new GoodsReceiptLine { GoodsReceiptId = receipt.Id, PurchaseOrderLineId = line.Id, Quantity = command.Quantity });
        await inventory.AdjustStock(line.ProductId, order.WarehouseId, command.Quantity, $"RECEIPT:{receipt.Id}");
        return order;
    });
}
