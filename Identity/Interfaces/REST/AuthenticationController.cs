using CeTe.Identity.Domain.Model.Aggregates;
using CeTe.Identity.Application.Results;
using System.Security.Claims;
using CeTe.Identity.Application.Services;
using CeTe.Identity.Interfaces.REST.Resources;
using CeTe.Identity.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
namespace CeTe.Identity.Interfaces.REST;

[ApiController]
[Route("api/v1/authentication")]
[Tags("Identity")]
public sealed class AuthenticationController(IIdentityCommandService commands) : ControllerBase
{
    [HttpPost("sign-up")]
    [EnableRateLimiting("auth")]
    public Task<RegisteredCompanyResult> Register(RegisterCompanyResource resource) => commands.Register(resource.ToCommand());

    [HttpPost("sign-in")]
    [EnableRateLimiting("auth")]
    public Task<SignedInResult> SignIn(SignInResource resource) => commands.SignIn(resource.ToCommand());

    [HttpPost("sign-out")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await commands.SignOut(Guid.Parse(User.FindFirstValue("jti")!));
        return NoContent();
    }
}
