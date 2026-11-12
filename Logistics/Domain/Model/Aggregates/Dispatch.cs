using CeTe.Shared.Domain.Model;
using CeTe.Shared.Domain.ValueObjects;

namespace CeTe.Logistics.Domain.Model.Aggregates;

public sealed class Dispatch : TenantEntity
{
    public string DriverName { get; set; } = ""; public string LicensePlate { get; set; } = ""; public string Status { get; private set; } = "PENDING"; public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public void Transition(string next)
    {
        Rules.Require((Status, next) is ("PENDING", "IN_TRANSIT") or ("IN_TRANSIT", "DELIVERED") or ("IN_TRANSIT", "INCIDENT") or ("INCIDENT", "IN_TRANSIT"), "Invalid dispatch transition.", 409);
        Status = next;
    }
}
