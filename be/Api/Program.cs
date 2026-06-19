using System.Text.Json;
using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using Lib.Infrastructure.App;
using Lib.Infrastructure.Redis;
using Lib.Infrastructure.Vcs;
using Lib.Modules.Auth;
using Lib.Modules.Projects;
using Lib.Modules.Releases;
using Lib.Modules.Storages;
using Lib.Modules.Storages.Entities.Schema;
using Lib.Modules.Tasks;
using Lib.Modules.Transfers;
using Lib.Modules.Users;
using Lib.Modules.Vcs;
using Lib.Modules.Vcs.Converters;
using Lib.Shared.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppInfrastructure(builder.Configuration);
builder.Services.AddVcsInfrastructure(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);

builder.Services.AddUsersModule();
builder.Services.AddAuthModule(builder.Configuration, builder.Environment);
builder.Services.AddStoragesModule();
builder.Services.AddTransfersModule();
builder.Services.AddProjectsModule();
builder.Services.AddVcsModule();
builder.Services.AddTasksModule();
builder.Services.AddReleasesModule();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    options.JsonSerializerOptions.Converters.Add(new SchemaFieldJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new HashIdJsonConverter());
});
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("NextJsDev", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>()
    .AddDbContextCheck<VcsDbContext>()
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();

app.UseForwardedHeaders();
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseCors("NextJsDev");

    const string openApiPattern = "/api/openapi/{documentName}.json";
    app.MapOpenApi(openApiPattern);
    app.MapScalarApiReference("/api/docs", options => { options.OpenApiRoutePattern = openApiPattern; });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapVcsModule();
app.MapHealthChecks("/api/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    })
    .AllowAnonymous()
    .WithTags("Health");

app.Run();