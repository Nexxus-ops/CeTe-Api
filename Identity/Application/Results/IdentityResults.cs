namespace CeTe.Identity.Application.Results;

public record RegisteredCompanyResult(Guid Id, string Username, Guid TenantId);
public record SignedInResult(Guid Id, string Username, string Role, Guid TenantId, string Token, DateTimeOffset ExpiresAt);
public record EmployeeResult(Guid Id, string Username, string Role);
public record UserResult(Guid Id, string Username, string Role, bool Active);
