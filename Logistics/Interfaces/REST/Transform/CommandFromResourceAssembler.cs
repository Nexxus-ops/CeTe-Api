using CeTe.Logistics.Domain.Model.Commands;
using CeTe.Logistics.Interfaces.REST.Resources;
namespace CeTe.Logistics.Interfaces.REST.Transform;

public static class CommandFromResourceAssembler
{
    public static CreateDispatchCommand ToCommand(this CreateDispatchResource resource) =>
        new(resource.DriverName, resource.LicensePlate, resource.SaleIds);

    public static ChangeDispatchStatusCommand ToCommand(this ChangeDispatchStatusResource resource) =>
        new(resource.Status);

    public static RegisterIncidentCommand ToCommand(this RegisterIncidentResource resource) =>
        new(resource.Description);
}
