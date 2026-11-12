using CeTe.Identity.Domain.Model.Aggregates;
using CeTe.Identity.Domain.Repositories;
using CeTe.Shared.Infrastructure.Persistence.EFC.Configuration;
using CeTe.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;
namespace CeTe.Identity.Infrastructure.Persistence.EFC.Repositories;

public sealed class IdentityRepository(CeTeDb db) : BaseRepository(db), IIdentityRepository
{
    // Global account lookup is limited to authentication and uniqueness checks.
    public Task<bool> AccountExists(string email) =>
        Database.Set<User>().IgnoreQueryFilters().AnyAsync(user => user.Username == email);
    public Task<bool> CompanyExists(string ruc) => Database.Set<Tenant>().AnyAsync(tenant => tenant.Ruc == ruc);
    public Task<User?> FindActiveUser(string email) =>
        Database.Set<User>().IgnoreQueryFilters().SingleOrDefaultAsync(user => user.Username == email && user.Active);
    public Task<Role> FindRole(string name) => Database.Set<Role>().SingleAsync(role => role.Name == name);
    public Task<UserSession?> FindSession(Guid id) => Find<UserSession>(id);
    public Task<List<User>> Users() => List<User>();
    public void AddTenant(Tenant tenant) => Add(tenant);
    public void AddUser(User user) => Add(user);
    public void AddRole(Role role) => Add(role);
    public void AddUserRole(UserRole role) => Add(role);
    public void AddSession(UserSession session) => Add(session);
}
