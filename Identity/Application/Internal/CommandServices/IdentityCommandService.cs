using CeTe.Identity.Application.Results;
using CeTe.Identity.Application.Services;
using CeTe.Identity.Domain.Model.Aggregates;
using CeTe.Identity.Domain.Model.Commands;
using CeTe.Identity.Domain.Repositories;
using CeTe.Inventory.Application.Services;
using CeTe.Shared.Application.Services;
using CeTe.Shared.Domain.Model;
using CeTe.Shared.Domain.Repositories;
using CeTe.Shared.Domain.ValueObjects;
namespace CeTe.Identity.Application.Internal.CommandServices;

public sealed class IdentityCommandService(
    IIdentityRepository repository, IUnitOfWork unitOfWork, ICurrentUser current,
    IInventoryFacade inventory, IPasswordService passwords, ITokenService tokens) : IIdentityCommandService
{
    private static string Email(string value)
    {
        var normalized = value.Trim().ToLowerInvariant();
        Rules.Require(System.Net.Mail.MailAddress.TryCreate(normalized, out var address)
            && address.Address == normalized, "Enter a valid email address.");
        return normalized;
    }

    private static void ValidatePassword(string password) =>
        Rules.Require(password.Length is >= 10 and <= 128, "Password must contain between 10 and 128 characters.");

    public async Task<RegisteredCompanyResult> Register(RegisterCompanyCommand command)
    {
        var email = Email(command.Username);
        ValidatePassword(command.Password);
        Rules.Require(!string.IsNullOrWhiteSpace(command.CompanyName)
            && !string.IsNullOrWhiteSpace(command.Address)
            && command.Ruc.Length == 11 && command.Ruc.All(char.IsAsciiDigit),
            "Company, address and an 11-digit RUC are required.");
        Rules.Require(!await repository.AccountExists(email) && !await repository.CompanyExists(command.Ruc),
            "The account or company is already registered.", 409);
        var tenant = new Tenant { Name = command.CompanyName.Trim(), Ruc = command.Ruc, Address = command.Address.Trim() };
        var user = new User { TenantId = tenant.Id, Username = email, Role = "Admin" };
        user.PasswordHash = passwords.Hash(user, command.Password);
        await unitOfWork.Transaction(async () =>
        {
            repository.AddTenant(tenant);
            repository.AddUser(user);
            inventory.ProvisionWarehouse(tenant.Id, tenant.Address);
            foreach (var name in new[] { "Admin", "Warehouse", "Sales", "Logistics" })
            {
                var role = new Role { TenantId = tenant.Id, Name = name };
                repository.AddRole(role);
                if (name == "Admin")
                    repository.AddUserRole(new UserRole { TenantId = tenant.Id, UserId = user.Id, RoleId = role.Id });
            }
            return true;
        });
        return new RegisteredCompanyResult(user.Id, user.Username, tenant.Id);
    }

    public async Task<SignedInResult> SignIn(SignInCommand command)
    {
        var user = await repository.FindActiveUser(command.Username.Trim().ToLowerInvariant());
        if (user == null || !passwords.Verify(user, command.Password))
            throw new BusinessException("Invalid credentials.", 401);
        var session = new UserSession
        {
            TenantId = user.TenantId,
            UserId = user.Id,
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(4)
        };
        repository.AddSession(session);
        await unitOfWork.Save();
        return new SignedInResult(user.Id, user.Username, user.Role, user.TenantId, tokens.Issue(user, session), session.ExpiresAt);
    }

    public async Task SignOut(Guid sessionId)
    {
        var session = await repository.FindSession(sessionId);
        if (session == null)
            return;
        session.Revoked = true;
        await unitOfWork.Save();
    }

    public async Task<EmployeeResult> CreateEmployee(CreateEmployeeCommand command)
    {
        var email = Email(command.Username);
        ValidatePassword(command.Password);
        Rules.Require(new[] { "Warehouse", "Sales", "Logistics" }.Contains(command.Role), "Invalid employee role.");
        Rules.Require(!await repository.AccountExists(email), "Account already registered.", 409);
        var user = new User { Username = email, Role = command.Role, TenantId = current.TenantId };
        user.PasswordHash = passwords.Hash(user, command.Password);
        var role = await repository.FindRole(command.Role);
        await unitOfWork.Transaction(async () =>
        {
            repository.AddUser(user);
            repository.AddUserRole(new UserRole { TenantId = current.TenantId, UserId = user.Id, RoleId = role.Id });
            return true;
        });
        return new EmployeeResult(user.Id, user.Username, user.Role);
    }
}
