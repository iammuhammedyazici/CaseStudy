using ECommerce.Order.Api.Services;
using ECommerce.Order.Api.Extensions;
using ECommerce.Observability;
using ECommerce.Order.Application;
using ECommerce.Order.Application.Abstractions;
using ECommerce.Order.Application.Abstractions.Persistence;
using ECommerce.Order.Infrastructure.Data;
using ECommerce.Order.Infrastructure.HealthChecks;
using ECommerce.Order.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

LoggingExtensions.ConfigureSerilog("OrderService", builder.Environment);
builder.Host.UseSerilog();



var connectionString = builder.Configuration.GetConnectionString("OrderDb")
    ?? builder.Configuration["ORDER_DB_CONNECTION"];

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddCustomMassTransit(builder.Configuration);

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddOrderApplication();

builder.Services.AddObservabilityBehaviors();

builder.Services.AddHealthChecks()
    .AddCheck<PostgresHealthCheck>("postgres");

builder.Services.AddCustomOpenTelemetry(builder.Configuration);
builder.Logging.AddCustomOpenTelemetry(builder.Configuration);

builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddCustomAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, ECommerce.Order.Api.Services.CurrentUserService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwagger();

var app = builder.Build();

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Service API v1");
    options.RoutePrefix = "swagger";
    options.InjectJavascript("/swagger/custom-auth.js");
});

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserContextMiddleware>();

app.MapHealthChecks("/health");

app.MapControllers();

app.MapPost("/dev/generate-token", (string userId = "user-123", string role = "Customer") =>
{
    var claims = new[]
    {
        new Claim("sub", userId),
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Role, role),
        new Claim("role", role),
        new Claim("email", $"{userId}@example.com"),
        new Claim("name", userId)
    };

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!));

    var token = new JwtSecurityToken(
        issuer: builder.Configuration["Jwt:Issuer"],
        audience: builder.Configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new
    {
        token = tokenString,
        userId = userId,
        role = role,
        expiresAt = token.ValidTo,
        instructions = @"Use this token in Swagger:
1. Click 'Authorize' button
2. Enter token (without 'Bearer' prefix)
3. Click 'Authorize' 
4. Now you can test authenticated endpoints"
    });
})
.WithName("GenerateDevToken")
.WithDescription("[DEV ONLY] Generate JWT token for testing")
.WithTags("Development");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();