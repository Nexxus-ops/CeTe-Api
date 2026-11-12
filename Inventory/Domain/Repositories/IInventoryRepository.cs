using CeTe.Inventory.Domain.Model.Aggregates;

namespace CeTe.Inventory.Domain.Repositories;

public interface IInventoryRepository
{
    Task<List<Product>> Products();
    Task<Product?> FindProduct(Guid id);
    Task<bool> SkuExists(string sku, Guid? exceptId);
    Task<List<Warehouse>> Warehouses();
    Task<Warehouse?> FindWarehouse(Guid id);
    Task<List<StockBalance>> Balances();
    Task<StockBalance?> FindBalance(Guid productId, Guid warehouseId);
    Task<List<InventoryMovement>> Movements(Guid productId);
    void AddProduct(Product product);
    void AddWarehouse(Warehouse warehouse);
    void AddBalance(StockBalance balance);
    void AddMovement(InventoryMovement movement);
}
