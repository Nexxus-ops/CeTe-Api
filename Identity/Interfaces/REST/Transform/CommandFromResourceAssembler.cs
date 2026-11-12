using CeTe.Identity.Domain.Model.Commands;
using CeTe.Identity.Interfaces.REST.Resources;
namespace CeTe.Identity.Interfaces.REST.Transform;

public static class CommandFromResourceAssembler
{
    public static RegisterCompanyCommand ToCommand(this RegisterCompanyResource resource) =>
        new(resource.Username, resource.Password, resource.CompanyName, resource.Ruc, resource.Address);
    public static SignInCommand ToCommand(this SignInResource resource) => new(resource.Username, resource.Password);
    public static CreateEmployeeCommand ToCommand(this CreateEmployeeResource resource) =>
        new(resource.Username, resource.Password, resource.Role);
}
