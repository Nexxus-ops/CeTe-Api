namespace CeTe.Identity.Domain.Model.Commands;

public record CreateEmployeeCommand(string Username, string Password, string Role);
