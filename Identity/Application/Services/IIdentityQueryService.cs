using CeTe.Identity.Domain.Model.Aggregates;
using CeTe.Identity.Application.Results;
using CeTe.Identity.Domain.Model.Queries;
namespace CeTe.Identity.Application.Services;

public interface IIdentityQueryService
{
    Task<UserResult[]> Users(GetUsersQuery query);
}
