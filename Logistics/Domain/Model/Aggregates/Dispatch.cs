using CeTe.Shared.Domain.Model;
using CeTe.Shared.Domain.ValueObjects;
namespace CeTe.Logistics.Domain.Model.Aggregates;

public sealed class Dispatch : TenantEntity
{
    public string DriverName { get; private set; } = "";
    public string LicensePlate { get; private set; } = "";
    public string Status { get; private set; } = "PENDING";
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    private Dispatch()
    {
    }

    public Dispatch(string driverName, string licensePlate)
    {
        Rules.Require(!string.IsNullOrWhiteSpace(driverName) && !string.IsNullOrWhiteSpace(licensePlate),
            "Driver, plate and sales are required.");
        DriverName = driverName.Trim();
        LicensePlate = licensePlate.Trim().ToUpperInvariant();
    }

    public void Transition(string next)
    {
        Rules.Require((Status, next) is ("PENDING", "IN_TRANSIT") or ("IN_TRANSIT", "DELIVERED")
            or ("IN_TRANSIT", "INCIDENT") or ("INCIDENT", "IN_TRANSIT"), "Invalid dispatch transition.", 409);
        Status = next;
    }
}
