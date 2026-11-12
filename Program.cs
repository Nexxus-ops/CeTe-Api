using CeTe.Shared.Application.Services;
using System.Text;
using System.Threading.RateLimiting;
using CeTe.Shared.Interfaces.REST;
using CeTe.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("CeTe");
if (connection?.StartsWith("postgres", StringComparison.OrdinalIgnoreCase) == true)
{
    var uri = new Uri(connection);
    var credentials = uri.UserInfo.Split(':', 2);
    connection = new Npgsql.NpgsqlConnectionStringBuilder { Host = uri.Host, Port = uri.IsDefaultPort ? 5432 : uri.Port, Database = uri.AbsolutePath.TrimStart('/'), Username = Uri.UnescapeDataString(credentials[0]), Password = Uri.UnescapeDataString(credentials[1]), SslMode = Npgsql.SslMode.Require }.ConnectionString;
}
var sqlite = builder.Configuration["Database:Provider"] == "Sqlite";
if (sqlite && !builder.Environment.IsDevelopment()) throw new InvalidOperationException("SQLite is only supported for local development.");
if (string.IsNullOrWhiteSpace(connection)) throw new InvalidOperationException("Configure ConnectionStrings__CeTe in the local .env or host environment.");
var key = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(key) || key.Length < 32) throw new InvalidOperationException("Configure a random Jwt__Key of at least 32 characters in the local .env.");
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CeTe.Identity.Infrastructure.Authentication.CurrentUser>();
builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
builder.Services.AddDbContext<CeTeDb>(options => { if (sqlite) options.UseSqlite(connection); else options.UseNpgsql(connection); });
builder.Services.AddScoped<CeTe.Shared.Domain.Repositories.IUnitOfWork>(services => services.GetRequiredService<CeTeDb>());
builder.Services.AddScoped<CeTe.Inventory.Application.Services.IInventoryFacade, CeTe.Inventory.Application.Internal.InventoryFacade>();
builder.Services.AddScoped<CeTe.Inventory.Domain.Repositories.IInventoryRepository, CeTe.Inventory.Infrastructure.Persistence.EFC.Repositories.InventoryRepository>();
builder.Services.AddScoped<CeTe.Sales.Domain.Repositories.ISalesRepository, CeTe.Sales.Infrastructure.Persistence.EFC.Repositories.SalesRepository>();
builder.Services.AddScoped<CeTe.Procurement.Domain.Repositories.IProcurementRepository, CeTe.Procurement.Infrastructure.Persistence.EFC.Repositories.ProcurementRepository>();
builder.Services.AddScoped<CeTe.Logistics.Domain.Repositories.ILogisticsRepository, CeTe.Logistics.Infrastructure.Persistence.EFC.Repositories.LogisticsRepository>();
builder.Services.AddScoped<CeTe.Inventory.Application.Services.IInventoryCommandService, CeTe.Inventory.Application.Internal.CommandServices.InventoryCommandService>();
builder.Services.AddScoped<CeTe.Inventory.Application.Services.IInventoryQueryService, CeTe.Inventory.Application.Internal.QueryServices.InventoryQueryService>();
builder.Services.AddScoped<CeTe.Sales.Application.Services.ISalesCommandService, CeTe.Sales.Application.Internal.CommandServices.SalesCommandService>();
builder.Services.AddScoped<CeTe.Sales.Application.Services.ISalesQueryService, CeTe.Sales.Application.Internal.QueryServices.SalesQueryService>();
builder.Services.AddScoped<CeTe.Procurement.Application.Services.IProcurementCommandService, CeTe.Procurement.Application.Internal.CommandServices.ProcurementCommandService>();
builder.Services.AddScoped<CeTe.Procurement.Application.Services.IProcurementQueryService, CeTe.Procurement.Application.Internal.QueryServices.ProcurementQueryService>();
builder.Services.AddScoped<CeTe.Logistics.Application.Services.ILogisticsCommandService, CeTe.Logistics.Application.Internal.CommandServices.LogisticsCommandService>();
builder.Services.AddScoped<CeTe.Logistics.Application.Services.ILogisticsQueryService, CeTe.Logistics.Application.Internal.QueryServices.LogisticsQueryService>();
builder.Services.AddScoped<CeTe.Sales.Application.Services.ISaleDispatchPolicy, CeTe.Logistics.Infrastructure.ACL.SaleDispatchPolicy>();
builder.Services.AddScoped<CeTe.Identity.Application.Services.ICompanyQueryService, CeTe.Identity.Infrastructure.Persistence.EFC.CompanyQueryService>();
builder.Services.AddScoped<CeTe.Identity.Domain.Repositories.IIdentityRepository, CeTe.Identity.Infrastructure.Persistence.EFC.Repositories.IdentityRepository>();
builder.Services.AddScoped<CeTe.Identity.Application.Services.IIdentityCommandService, CeTe.Identity.Application.Internal.CommandServices.IdentityCommandService>();
builder.Services.AddScoped<CeTe.Identity.Application.Services.IIdentityQueryService, CeTe.Identity.Application.Internal.QueryServices.IdentityQueryService>();
builder.Services.AddScoped<CeTe.Identity.Application.Services.IPasswordService, CeTe.Identity.Infrastructure.Authentication.PasswordService>();
builder.Services.AddScoped<CeTe.Identity.Application.Services.ITokenService, CeTe.Identity.Infrastructure.Authentication.TokenService>();
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context => new Microsoft.AspNetCore.Mvc.UnprocessableEntityObjectResult(
        new
        {
            message = "Invalid request."
        });
});
builder.Services.AddScoped<CeTe.Reporting.Application.Services.IReportingQueryService, CeTe.Reporting.Infrastructure.Persistence.EFC.ReportingQueryService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CeTe API", Version = "0.2.0", Description = "Inventory, procurement, sales receipts and dispatches. Sign in to obtain a Bearer token. Tenant isolation and role checks apply to protected endpoints." });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", Description = "Session token returned by authentication/sign-in." });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] });
});
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins((builder.Configuration["Cors:Origins"] ?? "http://localhost:5173").Split(',', StringSplitOptions.TrimEntries)).AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddHttpClient("rates", c => { c.BaseAddress = new Uri("https://open.er-api.com/"); c.Timeout = TimeSpan.FromSeconds(8); });
builder.Services.AddRateLimiter(options => options.AddPolicy("auth", context => RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions { PermitLimit = 20, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 })));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = true, ValidateAudience = true, ValidateIssuerSigningKey = true, ValidateLifetime = true, ValidIssuer = "CeTe", ValidAudience = "CeTe", IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), ClockSkew = TimeSpan.Zero };
    options.Events = new JwtBearerEvents { OnTokenValidated = CeTe.Identity.Infrastructure.Authentication.SessionValidator.Validate };
});
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
if (builder.Configuration.GetValue<bool>("Database:Initialize")) { using var scope = app.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<CeTeDb>(); await db.Database.EnsureCreatedAsync(); }
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/health", async (CeTeDb db) => { var healthy = await db.Database.CanConnectAsync(); return Results.Json(new { status = healthy ? "healthy" : "unavailable", service = "CeTe API", version = "0.2.0" }, statusCode: healthy ? 200 : 503); });
app.MapControllers();
app.Run();
