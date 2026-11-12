using CeTe.Identity.Domain.Model.Aggregates;
namespace CeTe.Identity.Domain.Repositories;

public interface IIdentityRepository
{
    Task<bool> AccountExists(string email);
    Task<bool> CompanyExists(string ruc);
    Task<User?> FindActiveUser(string email);
    Task<Role> FindRole(string name);
    Task<UserSession?> FindSession(Guid id);
    Task<List<User>> Users();
    void AddTenant(Tenant tenant);
    void AddUser(User user);
    void AddRole(Role role);
    void AddUserRole(UserRole role);
    void AddSession(UserSession session);
}
