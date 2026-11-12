namespace CeTe.Identity.Interfaces.REST.Resources;

public record RegisterCompanyResource(string Username, string Password, string CompanyName, string Ruc, string Address);
