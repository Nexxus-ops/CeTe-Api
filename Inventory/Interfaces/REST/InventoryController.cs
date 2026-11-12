using CeTe.Inventory.Domain.Model.Aggregates;
using CeTe.Inventory.Application.Results;
using CeTe.Inventory.Domain.Model.Queries;
using CeTe.Inventory.Application.Services;
using CeTe.Inventory.Interfaces.REST.Resources;
using CeTe.Inventory.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CeTe.Inventory.Interfaces.REST;

[ApiController]
[Route("api/v1/inventory")]
[Authorize]
[Tags("Inventory")]
public sealed class InventoryController(
    IInventoryCommandService commands,
    IInventoryQueryService queries) : ControllerBase
{
    [HttpGet]
    public Task<InventoryItemResult[]> GetAll() => queries.Inventory(new GetInventoryQuery());

    [HttpPost]
    [Authorize(Roles = "Admin,Warehouse")]
    public Task<InventoryItemResult> Create(SaveProductResource resource) => commands.SaveProduct(null, resource.ToCommand());

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Warehouse")]
    public Task<InventoryItemResult> Update(Guid id, SaveProductResource resource) => commands.SaveProduct(id, resource.ToCommand());

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public Task<ArchivedProductResult> Archive(Guid id) => commands.Archive(id);

    [HttpPost("{id:guid}/entries")]
    [Authorize(Roles = "Admin,Warehouse")]
    public Task<StockMovementResult> Receive(Guid id, MoveStockResource resource) => commands.Move(id, resource.ToCommand(), true);

    [HttpPost("{id:guid}/exits")]
    [Authorize(Roles = "Admin,Warehouse")]
    public Task<StockMovementResult> Issue(Guid id, MoveStockResource resource) => commands.Move(id, resource.ToCommand(), false);

    [HttpGet("{id:guid}/history")]
    public Task<IReadOnlyList<InventoryMovement>> History(Guid id) => queries.History(new GetProductHistoryQuery(id));
}
