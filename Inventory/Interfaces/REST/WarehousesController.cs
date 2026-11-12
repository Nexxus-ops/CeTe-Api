using CeTe.Inventory.Domain.Model.Queries;
using CeTe.Inventory.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CeTe.Inventory.Interfaces.REST;

[ApiController]
[Authorize]
[Route("api/v1/warehouses")]
[Tags("Inventory")]
public sealed class WarehousesController(IInventoryQueryService queries) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<CeTe.Inventory.Domain.Model.Aggregates.Warehouse>> GetAll() => queries.Warehouses(new GetWarehousesQuery());
}
