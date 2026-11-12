namespace CeTe.Logistics.Application.Results;

public record DispatchResult(Guid Id, string DriverName, string LicensePlate, string Status,
    DateTimeOffset CreatedAt, IEnumerable<Guid> SaleIds);
public record DispatchStatusResult(Guid Id, string DriverName, string LicensePlate, string Status, DateTimeOffset CreatedAt);
