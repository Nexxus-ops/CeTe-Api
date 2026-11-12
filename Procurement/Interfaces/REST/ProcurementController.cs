using CeTe.Procurement.Domain.Model.Aggregates;
using CeTe.Procurement.Application.Results;
using CeTe.Procurement.Domain.Model.Queries;
using CeTe.Procurement.Application.Services;
using CeTe.Procurement.Interfaces.REST.Resources;
using CeTe.Procurement.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CeTe.Procurement.Interfaces.REST;

[ApiController]
[Route("api/v1/purchases")]
[Authorize(Roles = "Admin,Warehouse")]
[Tags("Procurement")]
public sealed class ProcurementController(
    IProcurementCommandService commands,
    IProcurementQueryService queries) : ControllerBase
{
    [HttpGet]
    public Task<PurchaseResult[]> GetAll() => queries.Purchases(new GetPurchasesQuery());

    [HttpPost]
    public Task<PurchaseOrder> Create(CreatePurchaseResource resource) => commands.Purchase(resource.ToCommand());

    [HttpPost("{id:guid}/receive")]
    public Task<PurchaseOrder> Receive(Guid id, ReceivePurchaseResource resource) => commands.Receive(id, resource.ToCommand());
}
