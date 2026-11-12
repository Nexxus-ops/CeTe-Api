using CeTe.Inventory.Domain.Model.Aggregates;
using CeTe.Inventory.Application.Results;
using CeTe.Inventory.Domain.Model.Queries;
using CeTe.Inventory.Domain.Model.Commands;

namespace CeTe.Inventory.Application.Services;

public interface IInventoryQueryService
{
    Task<InventoryItemResult[]> Inventory(GetInventoryQuery query);
    Task<IReadOnlyList<InventoryMovement>> History(GetProductHistoryQuery query);
    Task<IReadOnlyList<Warehouse>> Warehouses(GetWarehousesQuery query);
}
