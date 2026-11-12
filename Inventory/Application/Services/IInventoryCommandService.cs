using CeTe.Inventory.Domain.Model.Aggregates;
using CeTe.Inventory.Application.Results;
using CeTe.Inventory.Domain.Model.Commands;
using CeTe.Inventory.Application.Services;
namespace CeTe.Inventory.Application.Services;

public interface IInventoryCommandService
{
    Task<InventoryItemResult> SaveProduct(Guid? id, SaveProductCommand input);
    Task<StockMovementResult> Move(Guid id, MoveStockCommand input, bool receive);
    Task<ArchivedProductResult> Archive(Guid id);
}
