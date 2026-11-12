using CeTe.Inventory.Application.Results;
using CeTe.Inventory.Domain.Model.ValueObjects;
using CeTe.Inventory.Domain.Model.Aggregates;
using CeTe.Inventory.Domain.Model.Commands;
using CeTe.Inventory.Domain.Repositories;
using CeTe.Shared.Application.Services;
using CeTe.Shared.Domain.Model;
using CeTe.Shared.Domain.Repositories;
using CeTe.Shared.Domain.ValueObjects;
using CeTe.Inventory.Application.Services;

namespace CeTe.Inventory.Application.Internal.CommandServices;

public sealed class InventoryCommandService(IInventoryRepository repository, IUnitOfWork unitOfWork, IInventoryFacade inventory) : IInventoryCommandService
{
    public Task<InventoryItemResult> SaveProduct(Guid? id, SaveProductCommand input) => unitOfWork.Transaction<InventoryItemResult>(async () =>
    {
        Rules.Require(!string.IsNullOrWhiteSpace(input.Sku) && !string.IsNullOrWhiteSpace(input.Name), "SKU and name are required.");
        Rules.Require(input.Quantity >= 0 && input.UnitPrice >= 0 && input.MinimumStock >= 0, "Invalid quantities or price.");
        var sku = new Sku(input.Sku);
        Rules.Require(!await repository.SkuExists(sku.Value, id), "SKU already exists.", 409);
        var product = id.HasValue ? await repository.FindProduct(id.Value) ?? throw new BusinessException("Resource not found.", 404) : new Product();
        if (!id.HasValue)
            repository.AddProduct(product);
        product.UpdateDetails(sku, input.Name, new Money(input.UnitPrice), input.MinimumStock);
        // Initial stock is an auditable opening movement. Later adjustments use movement endpoints.
        if (!id.HasValue && input.Quantity > 0)
        {
            var warehouseId = (await inventory.GetWarehouse(input.WarehouseId)).Id;
            await inventory.AdjustStock(product.Id, warehouseId, input.Quantity, "OPENING");
        }
        await unitOfWork.Save();
        return new InventoryItemResult(product.Id, product.Sku, product.Name, product.UnitPrice, product.MinimumStock,
            (await repository.Balances()).Where(balance => balance.ProductId == product.Id).Sum(balance => balance.Quantity));
    });
    public Task<StockMovementResult> Move(Guid id, MoveStockCommand input, bool receive) => unitOfWork.Transaction<StockMovementResult>(async () =>
    {
        await inventory.GetProduct(id);
        Rules.Require(input.Quantity > 0, "Quantity must be positive.");
        Rules.Require(!string.IsNullOrWhiteSpace(input.Reason), "A movement reason is required.");
        var warehouseId = (await inventory.GetWarehouse(input.WarehouseId)).Id;
        var quantity = await inventory.AdjustStock(id, warehouseId, receive ? input.Quantity : -input.Quantity, input.Reason.Trim());
        return new StockMovementResult(id, quantity);
    });
    public Task<ArchivedProductResult> Archive(Guid id) => unitOfWork.Transaction<ArchivedProductResult>(async () =>
    {
        var product = await repository.FindProduct(id) ?? throw new BusinessException("Resource not found.", 404);
        product.Archive((await repository.Balances()).Any(balance => balance.ProductId == id && balance.Quantity != 0));
        return new ArchivedProductResult(product.Id);
    });
}
