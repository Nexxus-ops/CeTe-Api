namespace CeTe.Identity.Application.Services;

public record CompanySnapshot(string Name, string Ruc);
public interface ICompanyQueryService
{
    Task<CompanySnapshot> CurrentCompany();
}
