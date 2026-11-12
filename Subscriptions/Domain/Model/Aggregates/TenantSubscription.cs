using CeTe.Shared.Domain.Model;

namespace CeTe.Subscriptions.Domain.Model.Aggregates;

public sealed class TenantSubscription : TenantEntity
{
    public Guid SubscriptionPlanId
    {
        get; set;
    }
    public string Status { get; set; } = "TRIAL";
}
