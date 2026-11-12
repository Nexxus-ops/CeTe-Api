namespace CeTe.Logistics.Interfaces.REST.Resources;

public record CreateDispatchResource(string DriverName, string LicensePlate, List<Guid> SaleIds);
