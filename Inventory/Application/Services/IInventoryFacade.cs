namespace CeTe.Inventory.Application.Services;

public record ProductSnapshot(Guid Id, string Sku, string Name, decimal UnitPrice, bool Active);
public record WarehouseSnapshot(Guid Id, string Name, string Address);

public interface IInventoryFacade
{
    Task<ProductSnapshot> GetProduct(Guid id);
    Task<WarehouseSnapshot> GetWarehouse(Guid? id);
    Task<decimal> AdjustStock(Guid productId, Guid warehouseId, decimal quantity, string reason);
    void ProvisionWarehouse(Guid tenantId, string address);
}
