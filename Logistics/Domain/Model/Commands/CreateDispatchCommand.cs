namespace CeTe.Logistics.Domain.Model.Commands;

public record CreateDispatchCommand(string DriverName, string LicensePlate, List<Guid> SaleIds);
