using CeTe.Identity.Domain.Model.Aggregates;
using CeTe.Identity.Application.Results;
using CeTe.Identity.Domain.Model.Queries;
using CeTe.Identity.Application.Services;
using CeTe.Identity.Interfaces.REST.Resources;
using CeTe.Identity.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CeTe.Identity.Interfaces.REST;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/users")]
[Tags("Identity")]
public sealed class UsersController(IIdentityCommandService commands, IIdentityQueryService queries) : ControllerBase
{
    [HttpGet]
    public Task<UserResult[]> GetAll() => queries.Users(new GetUsersQuery());
    [HttpPost]
    public Task<EmployeeResult> Create(CreateEmployeeResource resource) => commands.CreateEmployee(resource.ToCommand());
}
