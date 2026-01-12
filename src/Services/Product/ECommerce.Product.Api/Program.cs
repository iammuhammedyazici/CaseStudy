using ECommerce.Product.Api.Extensions;
using ECommerce.Product.Application.Abstractions.Persistence;
using ECommerce.Product.Application.Abstractions.Search;
using ECommerce.Product.Infrastructure.Data;
using ECommerce.Product.Infrastructure.Repositories;
using ECommerce.Product.Infrastructure.Search;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Nest;
using ECommerce.Observability;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

LoggingExtensions.ConfigureSerilog("ProductService", builder.Environment);
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwagger();
builder.Services.AddHealthChecks();

var connectionString = builder.Configuration.GetConnectionString("ProductDb")
    ?? builder.Configuration["PRODUCT_DB_CONNECTION"];

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseNpgsql(connectionString));

var elasticsearchUrl = builder.Configuration["Elasticsearch:Url"]
    ?? throw new InvalidOperationException("Elasticsearch:Url configuration is missing");
var settings = new ConnectionSettings(new Uri(elasticsearchUrl))
    .DefaultIndex("products");

builder.Services.AddSingleton<IElasticClient>(new ElasticClient(settings));
builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    var esService = scope.ServiceProvider.GetRequiredService<IElasticsearchService>();
    await context.Database.MigrateAsync();
    await ProductSeeder.SeedAsync(context, esService);
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Service API v1");
    c.RoutePrefix = "swagger";
});

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

app.MapHealthChecks("/health");
app.UseAuthorization();
app.MapControllers();
app.Run();
