using CeTe.Shared.Domain.Model;

namespace CeTe.Notifications.Domain.Model.Aggregates;

public sealed class Notification : TenantEntity
{
    public string Message { get; set; } = ""; public string EventKey { get; set; } = ""; public bool IsRead
    {
        get; set;
    }
}
