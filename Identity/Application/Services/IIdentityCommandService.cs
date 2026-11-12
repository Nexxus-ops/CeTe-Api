using CeTe.Identity.Domain.Model.Aggregates;
using CeTe.Identity.Application.Results;
using CeTe.Identity.Domain.Model.Commands;
namespace CeTe.Identity.Application.Services;

public interface IIdentityCommandService
{
    Task<RegisteredCompanyResult> Register(RegisterCompanyCommand command);
    Task<SignedInResult> SignIn(SignInCommand command);
    Task SignOut(Guid sessionId);
    Task<EmployeeResult> CreateEmployee(CreateEmployeeCommand command);
}
