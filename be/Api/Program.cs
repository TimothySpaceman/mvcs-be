using System.Text.Json.Serialization;
using Lib.Infrastructure.App;
using Lib.Infrastructure.Redis;
using Lib.Infrastructure.Vcs;
using Lib.Modules.Auth;
using Lib.Modules.Projects;
using Lib.Modules.Storages;
using Lib.Modules.Transfers;
using Lib.Modules.Users;
using Lib.Modules.Vcs;
using Lib.Shared.Middleware;
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

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
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

app.Run();