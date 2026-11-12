using CeTe.Inventory.Domain.Model.Aggregates;
using CeTe.Inventory.Domain.Repositories;
using CeTe.Shared.Infrastructure.Persistence.EFC.Configuration;
using CeTe.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace CeTe.Inventory.Infrastructure.Persistence.EFC.Repositories;

public sealed class InventoryRepository(CeTeDb db) : BaseRepository(db), IInventoryRepository
{
    public Task<List<Product>> Products() => List<Product>(p => p.Active);
    public Task<Product?> FindProduct(Guid id) => Find<Product>(id);
    public Task<bool> SkuExists(string sku, Guid? exceptId) => ExistsSku(sku, exceptId);
    public Task<List<Warehouse>> Warehouses() => List<Warehouse>();
    public Task<Warehouse?> FindWarehouse(Guid id) => Find<Warehouse>(id);
    public Task<List<StockBalance>> Balances() => List<StockBalance>();
    public Task<StockBalance?> FindBalance(Guid productId, Guid warehouseId) => Balance(productId, warehouseId);
    public Task<List<InventoryMovement>> Movements(Guid productId) => List<InventoryMovement>(m => m.ProductId == productId);
    public void AddProduct(Product product) => Add(product);
    public void AddWarehouse(Warehouse warehouse) => Add(warehouse);
    public void AddBalance(StockBalance balance) => Add(balance);
    public void AddMovement(InventoryMovement movement) => Add(movement);

    private async Task<bool> ExistsSku(string sku, Guid? exceptId) =>
        (await List<Product>(product => product.Sku == sku && product.Id != exceptId)).Count != 0;
    private async Task<StockBalance?> Balance(Guid productId, Guid warehouseId) =>
        (await List<StockBalance>(balance => balance.ProductId == productId && balance.WarehouseId == warehouseId)).SingleOrDefault();
}
