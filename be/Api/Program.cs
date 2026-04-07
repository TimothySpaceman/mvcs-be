using Lib.Infrastructure.App;
using Lib.Infrastructure.Redis;
using Lib.Modules.Auth;
using Lib.Modules.Users;
using Lib.Shared.Middleware;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppInfrastructure(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);

builder.Services.AddUsersModule();
builder.Services.AddAuthModule(builder.Configuration, builder.Environment);

builder.Services.AddControllers();
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
    app.MapScalarApiReference("/api/docs", options =>
    {
        options.OpenApiRoutePattern = openApiPattern;
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();