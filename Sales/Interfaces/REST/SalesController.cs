using CeTe.Sales.Domain.Model.Aggregates;
using CeTe.Sales.Application.Results;
using CeTe.Sales.Domain.Model.Queries;
using CeTe.Sales.Application.Services;
using CeTe.Sales.Interfaces.REST.Resources;
using CeTe.Sales.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CeTe.Sales.Interfaces.REST;

[ApiController]
[Route("api/v1/sales")]
[Authorize]
[Tags("Sales")]
public sealed class SalesController(
    ISalesCommandService commands,
    ISalesQueryService queries) : ControllerBase
{
    [HttpGet]
    public Task<SaleResult[]> GetAll() => queries.Sales(new GetSalesQuery());

    [HttpPost]
    [Authorize(Roles = "Admin,Sales")]
    public Task<SaleResult> Create(ConfirmSaleResource resource) => commands.Checkout(resource.ToCommand());

    [HttpPatch("{id:guid}/cancel")]
    [Authorize(Roles = "Admin")]
    public Task<SaleResult> Cancel(Guid id) => commands.CancelSale(id);

    [HttpGet("{id:guid}/receipt")]
    [Authorize(Roles = "Admin,Sales")]
    public Task<ReceiptResult> Receipt(Guid id) => queries.Receipt(new GetReceiptBySaleIdQuery(id));
}
