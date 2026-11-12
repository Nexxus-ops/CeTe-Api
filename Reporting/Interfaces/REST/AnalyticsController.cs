using CeTe.Reporting.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CeTe.Reporting.Interfaces.REST;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/analytics")]
[Tags("Analytics")]
public sealed class AnalyticsController(IReportingQueryService queries) : ControllerBase
{
    [HttpGet("dashboard")]
    public Task<DashboardResult> Dashboard() => queries.Dashboard();
}
