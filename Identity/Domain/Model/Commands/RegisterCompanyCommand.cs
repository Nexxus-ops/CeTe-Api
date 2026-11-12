namespace CeTe.Identity.Domain.Model.Commands;

public record RegisterCompanyCommand(string Username, string Password, string CompanyName, string Ruc, string Address);
