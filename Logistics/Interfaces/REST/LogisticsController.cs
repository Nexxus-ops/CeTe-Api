using CeTe.Logistics.Domain.Model.Aggregates;
using CeTe.Logistics.Application.Results;
using CeTe.Logistics.Domain.Model.Queries;
using CeTe.Logistics.Application.Services;
using CeTe.Logistics.Interfaces.REST.Resources;
using CeTe.Logistics.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CeTe.Logistics.Interfaces.REST;

[ApiController]
[Route("api/v1/dispatches")]
[Authorize]
[Tags("Logistics")]
public sealed class LogisticsController(
    ILogisticsCommandService commands,
    ILogisticsQueryService queries) : ControllerBase
{
    [HttpGet]
    public Task<DispatchResult[]> GetAll() => queries.Dispatches(new GetDispatchesQuery());

    [HttpPost]
    [Authorize(Roles = "Admin,Logistics")]
    public Task<DispatchResult> Create(CreateDispatchResource resource) => commands.CreateDispatch(resource.ToCommand());

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Logistics")]
    public Task<DispatchStatusResult> ChangeStatus(Guid id, ChangeDispatchStatusResource resource) => commands.SetDispatchStatus(id, resource.ToCommand());

    [HttpPost("{id:guid}/incidents")]
    [Authorize(Roles = "Admin,Logistics")]
    public Task<DispatchIncident> Incident(Guid id, RegisterIncidentResource resource) => commands.Incident(id, resource.ToCommand());
}
