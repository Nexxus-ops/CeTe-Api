using CeTe.Inventory.Application.Results;
using CeTe.Inventory.Application.Services;
using CeTe.Inventory.Domain.Model.Aggregates;
using CeTe.Inventory.Domain.Model.Queries;
using CeTe.Inventory.Domain.Repositories;
using CeTe.Shared.Domain.Model;
namespace CeTe.Inventory.Application.Internal.QueryServices;

public sealed class InventoryQueryService(IInventoryRepository repository) : IInventoryQueryService
{
    public async Task<InventoryItemResult[]> Inventory(GetInventoryQuery query)
    {
        var balances = await repository.Balances();
        var products = await repository.Products();
        return products.Select(product => new InventoryItemResult(product.Id, product.Sku, product.Name,
            product.UnitPrice, product.MinimumStock,
            balances.Where(balance => balance.ProductId == product.Id).Sum(balance => balance.Quantity))).ToArray();
    }

    public async Task<IReadOnlyList<InventoryMovement>> History(GetProductHistoryQuery query)
    {
        if (await repository.FindProduct(query.ProductId) == null)
            throw new BusinessException("Resource not found.", 404);
        return (await repository.Movements(query.ProductId)).OrderByDescending(movement => movement.CreatedAt).ToArray();
    }

    public async Task<IReadOnlyList<Warehouse>> Warehouses(GetWarehousesQuery query) => await repository.Warehouses();
}
