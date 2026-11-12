using CeTe.Identity.Domain.Model.Aggregates;
using CeTe.Identity.Application.Results;
using CeTe.Identity.Domain.Model.Queries;
using CeTe.Identity.Application.Services;
using CeTe.Identity.Domain.Repositories;
namespace CeTe.Identity.Application.Internal.QueryServices;

public sealed class IdentityQueryService(IIdentityRepository repository) : IIdentityQueryService
{
    public async Task<UserResult[]> Users(GetUsersQuery query) =>
        (await repository.Users()).Select(user => new UserResult(user.Id, user.Username, user.Role, user.Active)).ToArray();
}
