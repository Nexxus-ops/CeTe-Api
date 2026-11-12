namespace CeTe.Shared.Application.Services;

public interface ICurrentUser
{
    Guid TenantId
    {
        get;
    }
    Guid UserId
    {
        get;
    }
    string Username
    {
        get;
    }
}
