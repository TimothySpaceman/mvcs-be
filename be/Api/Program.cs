using Lib.Infrastructure.App;
using Lib.Modules.Auth;
using Lib.Modules.Users;
using Lib.Shared.Middleware;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppInfrastructure(builder.Configuration);
builder.Services.AddUsersModule();
builder.Services.AddAuthModule(builder.Configuration, builder.Environment);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseForwardedHeaders();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    const string openApiPattern = "/api/openapi/{documentName}.json";
    app.MapOpenApi(openApiPattern);
    app.MapScalarApiReference("/api/docs", options =>
    {
        options.OpenApiRoutePattern = openApiPattern;
    });
}

app.MapControllers();

app.Run();