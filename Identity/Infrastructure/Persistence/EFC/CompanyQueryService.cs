using CeTe.Identity.Application.Services;
using CeTe.Identity.Domain.Model.Aggregates;
using CeTe.Shared.Application.Services;
using CeTe.Shared.Domain.Model;
using CeTe.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;
namespace CeTe.Identity.Infrastructure.Persistence.EFC;

public sealed class CompanyQueryService(CeTeDb db, ICurrentUser user) : ICompanyQueryService
{
    public async Task<CompanySnapshot> CurrentCompany()
    {
        var tenant = await db.Set<Tenant>().SingleOrDefaultAsync(company => company.Id == user.TenantId)
            ?? throw new BusinessException("Resource not found.", 404);
        return new CompanySnapshot(tenant.Name, tenant.Ruc);
    }
}
