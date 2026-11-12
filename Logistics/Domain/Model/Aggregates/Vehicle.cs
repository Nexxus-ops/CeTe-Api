using CeTe.Shared.Domain.Model;

namespace CeTe.Logistics.Domain.Model.Aggregates;

public sealed class Vehicle : TenantEntity
{
    public string LicensePlate { get; set; } = ""; public decimal Capacity
    {
        get; set;
    }
}
