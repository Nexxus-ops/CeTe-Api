using CeTe.Identity.Domain.Model.Aggregates;
namespace CeTe.Identity.Application.Services;

public interface IPasswordService
{
    string Hash(User user, string password);
    bool Verify(User user, string password);
}
public interface ITokenService
{
    string Issue(User user, UserSession session);
}
