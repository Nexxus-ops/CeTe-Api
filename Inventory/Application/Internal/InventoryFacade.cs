using CeTe.Inventory.Application.Services;
using CeTe.Inventory.Domain.Model.Aggregates;
using CeTe.Inventory.Domain.Repositories;
using CeTe.Shared.Application.Services;
using CeTe.Shared.Domain.Model;

namespace CeTe.Inventory.Application.Internal;

// Mutations participate in the caller's unit of work.
public sealed class InventoryFacade(IInventoryRepository repository, ICurrentUser user) : IInventoryFacade
{
    public async Task<ProductSnapshot> GetProduct(Guid id)
    {
        var product = await repository.FindProduct(id) ?? throw new BusinessException("Resource not found.", 404);
        return new ProductSnapshot(product.Id, product.Sku, product.Name, product.UnitPrice, product.Active);
    }

    public async Task<WarehouseSnapshot> GetWarehouse(Guid? id)
    {
        var warehouse = id.HasValue
            ? await repository.FindWarehouse(id.Value)
            : (await repository.Warehouses()).FirstOrDefault();
        if (warehouse == null)
            throw new BusinessException("Warehouse not found.", 404);
        return new WarehouseSnapshot(warehouse.Id, warehouse.Name, warehouse.Address);
    }

    public async Task<decimal> AdjustStock(Guid productId, Guid warehouseId, decimal quantity, string reason)
    {
        var balance = await repository.FindBalance(productId, warehouseId);
        if (balance == null)
        {
            balance = new StockBalance { ProductId = productId, WarehouseId = warehouseId };
            repository.AddBalance(balance);
        }
        if (quantity > 0)
            balance.Receive(quantity);
        else
            balance.Remove(-quantity);
        repository.AddMovement(new InventoryMovement
        {
            ProductId = productId,
            WarehouseId = warehouseId,
            UserId = user.UserId,
            Quantity = quantity,
            Reason = reason
        });
        return balance.Quantity;
    }

    public void ProvisionWarehouse(Guid tenantId, string address) =>
        repository.AddWarehouse(new Warehouse { TenantId = tenantId, Name = "Main warehouse", Address = address });
}
