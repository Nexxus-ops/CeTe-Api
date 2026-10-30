using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using CeTe.Application;
using CeTe.Domain;
using CeTe.Infrastructure;
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
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
builder.Services.AddDbContext<CeTeDb>(options => { if (sqlite) options.UseSqlite(connection); else options.UseNpgsql(connection); });
builder.Services.AddScoped<ICeTeRepository>(s => s.GetRequiredService<CeTeDb>());
builder.Services.AddScoped<Operations>();
builder.Services.AddScoped<Authentication>();
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
    options.Events = new JwtBearerEvents { OnTokenValidated = async context => { var principal = context.Principal!; if (!Guid.TryParse(principal.FindFirst("jti")?.Value, out var sessionId) || !Guid.TryParse(principal.FindFirst("tenant_id")?.Value, out var tenantId)) { context.Fail("Invalid session."); return; } var db = context.HttpContext.RequestServices.GetRequiredService<CeTeDb>(); var session = await db.Set<UserSession>().IgnoreQueryFilters().SingleOrDefaultAsync(s => s.Id == sessionId && s.TenantId == tenantId); if (session == null || session.Revoked || session.ExpiresAt <= DateTimeOffset.UtcNow) context.Fail("Session expired."); } };
});
builder.Services.AddAuthorization();
var app = builder.Build();
app.Use(async (context, next) => { try { await next(); } catch (BusinessException e) { context.Response.StatusCode = e.Status; await context.Response.WriteAsJsonAsync(new { message = ErrorLanguage.Translate(e.Message, context) }); } catch (Exception e) when (IsConcurrencyConflict(e)) { context.Response.StatusCode = 409; await context.Response.WriteAsJsonAsync(new { message = ErrorLanguage.Translate("Concurrent operation. Refresh and retry.", context) }); } catch (DbUpdateException) { context.Response.StatusCode = 409; await context.Response.WriteAsJsonAsync(new { message = ErrorLanguage.Translate("The operation conflicts with current data. Refresh and retry.", context) }); } catch (Exception e) when (e is not OperationCanceledException) { app.Logger.LogError(e, "Request failed"); context.Response.StatusCode = 500; await context.Response.WriteAsJsonAsync(new { message = ErrorLanguage.Translate("The operation could not be completed.", context) }); } });
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
if (builder.Configuration.GetValue<bool>("Database:Initialize")) { using var scope = app.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<CeTeDb>(); await db.Database.EnsureCreatedAsync(); }
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/health", async (CeTeDb db) => { var healthy = await db.Database.CanConnectAsync(); return Results.Json(new { status = healthy ? "healthy" : "unavailable", service = "CeTe API", version = "0.2.0" }, statusCode: healthy ? 200 : 503); });
var auth = app.MapGroup("/api/v1/authentication").WithTags("Identity");
auth.MapPost("/sign-up", (RegisterInput body, Authentication service) => service.Register(body)).RequireRateLimiting("auth");
auth.MapPost("/sign-in", (LoginInput body, Authentication service) => service.Login(body)).RequireRateLimiting("auth");
auth.MapPost("/sign-out", async (ClaimsPrincipal principal, Authentication service) => { await service.Logout(principal); return Results.NoContent(); }).RequireAuthorization();
var api = app.MapGroup("/api/v1").RequireAuthorization();
api.MapGet("/inventory", (Operations ops) => ops.Inventory()).WithTags("Inventory");
api.MapPost("/inventory", (InventoryInput body, Operations ops) => ops.SaveProduct(null, body)).RequireAuthorization(p => p.RequireRole("Admin", "Warehouse")).WithTags("Inventory");
api.MapPut("/inventory/{id:guid}", (Guid id, InventoryInput body, Operations ops) => ops.SaveProduct(id, body)).RequireAuthorization(p => p.RequireRole("Admin", "Warehouse")).WithTags("Inventory");
api.MapDelete("/inventory/{id:guid}", (Guid id, Operations ops) => ops.Archive(id)).RequireAuthorization(p => p.RequireRole("Admin")).WithTags("Inventory");
api.MapPost("/inventory/{id:guid}/entries", (Guid id, MovementInput body, Operations ops) => ops.Move(id, body, true)).RequireAuthorization(p => p.RequireRole("Admin", "Warehouse")).WithTags("Inventory");
api.MapPost("/inventory/{id:guid}/exits", (Guid id, MovementInput body, Operations ops) => ops.Move(id, body, false)).RequireAuthorization(p => p.RequireRole("Admin", "Warehouse")).WithTags("Inventory");
api.MapGet("/inventory/{id:guid}/history", (Guid id, Operations ops) => ops.History(id)).WithTags("Inventory");
api.MapGet("/sales", (Operations ops) => ops.Sales()).WithTags("Sales");
api.MapPost("/sales", (SaleInput body, Operations ops) => ops.Checkout(body)).RequireAuthorization(p => p.RequireRole("Admin", "Sales")).WithTags("Sales");
api.MapPatch("/sales/{id:guid}/cancel", (Guid id, Operations ops) => ops.CancelSale(id)).RequireAuthorization(p => p.RequireRole("Admin")).WithTags("Sales");
api.MapGet("/sales/{id:guid}/receipt", (Guid id, Operations ops) => ops.Receipt(id)).RequireAuthorization(p => p.RequireRole("Admin", "Sales")).WithTags("Sales");
api.MapGet("/dispatches", (Operations ops) => ops.Dispatches()).WithTags("Logistics");
api.MapPost("/dispatches", (DispatchInput body, Operations ops) => ops.CreateDispatch(body)).RequireAuthorization(p => p.RequireRole("Admin", "Logistics")).WithTags("Logistics");
api.MapPut("/dispatches/{id:guid}", (Guid id, StatusInput body, Operations ops) => ops.SetDispatchStatus(id, body.Status)).RequireAuthorization(p => p.RequireRole("Admin", "Logistics")).WithTags("Logistics");
api.MapPost("/dispatches/{id:guid}/incidents", (Guid id, IncidentInput body, Operations ops) => ops.Incident(id, body.Description)).RequireAuthorization(p => p.RequireRole("Admin", "Logistics")).WithTags("Logistics");
api.MapGet("/purchases", (Operations ops) => ops.Purchases()).RequireAuthorization(p => p.RequireRole("Admin", "Warehouse")).WithTags("Procurement");
api.MapPost("/purchases", (PurchaseInput body, Operations ops) => ops.Purchase(body)).RequireAuthorization(p => p.RequireRole("Admin", "Warehouse")).WithTags("Procurement");
api.MapPost("/purchases/{id:guid}/receive", (Guid id, ReceiveInput body, Operations ops) => ops.Receive(id, body.Quantity)).RequireAuthorization(p => p.RequireRole("Admin", "Warehouse")).WithTags("Procurement");
api.MapGet("/analytics/dashboard", (Operations ops) => ops.Dashboard()).RequireAuthorization(p => p.RequireRole("Admin")).WithTags("Analytics");
api.MapGet("/warehouses", (ICeTeRepository repository) => repository.List<Warehouse>()).WithTags("Inventory");
api.MapGet("/users", async (ICeTeRepository repository) => (await repository.List<User>()).Select(u => new { u.Id, u.Username, u.Role, u.Active })).RequireAuthorization(p => p.RequireRole("Admin")).WithTags("Identity");
api.MapPost("/users", (EmployeeInput body, Authentication service) => service.Employee(body)).RequireAuthorization(p => p.RequireRole("Admin")).WithTags("Identity");
api.MapGet("/integrations/exchange-rates", async (IHttpClientFactory factory) => { try { var response = await factory.CreateClient("rates").GetFromJsonAsync<System.Text.Json.JsonElement>("v6/latest/USD"); return Results.Ok(new { source = "ExchangeRate-API", sourceUrl = "https://www.exchangerate-api.com", baseCurrency = "USD", pen = response.GetProperty("rates").GetProperty("PEN").GetDecimal(), updatedAt = response.GetProperty("time_last_update_utc").GetString() }); } catch (Exception e) when (e is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException) { return Results.Problem("Exchange rate provider is unavailable.", statusCode: 503); } }).WithTags("Integrations");
app.Run();

static bool IsConcurrencyConflict(Exception error)
{
    for (Exception? current = error; current != null; current = current.InnerException)
        if (current is Npgsql.PostgresException { SqlState: "40001" or "40P01" })
            return true;
    return false;
}
